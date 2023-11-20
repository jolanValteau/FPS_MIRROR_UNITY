using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead; }
		protected set { _isDead = value; }
	}
	[SerializeField]
	private float maxHealth = 100f;
	
	[SyncVar]
	private float currentHealth;
	
	public int kills;
	public int deaths;
	
	[SerializeField]
	private Behaviour[] disableOnDeath;
	
	[SerializeField]
	private GameObject[] disableGameObjectOnDeath;
	
	private bool[] wasEnabledOnStart;
	
	[SerializeField]
	private GameObject deathEffects;
	[SerializeField]
	private GameObject spawnEffects;
	
	private bool firstSetup = true;
	
	public void Setup()
	{
		if (isLocalPlayer) 
		{
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
		}

		CmdBroadcastNewPlayerSetup();
	}
	
	[Command(requiresAuthority = false)]
	private void CmdBroadcastNewPlayerSetup()
	{
		RpcSetupPlayerOnAllClients();
	}
	
	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		if(firstSetup)
		{
			wasEnabledOnStart = new bool[disableOnDeath.Length];
			for (int i = 0; i < disableOnDeath.Length; i++)
			{
				wasEnabledOnStart[i] = disableOnDeath[i].enabled;
			}
			
			firstSetup = false;
		}
		SetDefault();
	}
	
	public void SetDefault()
	{
		isDead = false;
		currentHealth = maxHealth;

		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = wasEnabledOnStart[i];
		}

		for (int i = 0; i < disableGameObjectOnDeath.Length; i++) 
		{
			disableGameObjectOnDeath[i].SetActive(true);
		}
		
		Collider col = GetComponent<Collider>();
		if (col!= null)
		{
			col.enabled = true;
		}
		
		
		//Effects
		GameObject _gfxIns = Instantiate(spawnEffects, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
	}
	
	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
		Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;
		
		yield return new WaitForSeconds(0.1f);
		
		Setup();	
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			RpcTakeDamage(91, "Joueur");
		}
	}
	
	[ClientRpc]
	public void RpcTakeDamage(float amount, string sourceID)
	{
		if(isDead)
		{
			return;
		}
		currentHealth -= amount;
		Debug.Log(transform.name + " a maintenant : " + currentHealth + " pv sur " + maxHealth);
		if (currentHealth <= 0)
		{
			Die(sourceID);
		}
	}
	
	private void Die(string sourceID)
	{
		isDead = true;
		
		Player sourcePlayer = GameManager.GetPlayer(sourceID);
		if(sourcePlayer != null)
		{
			sourcePlayer.kills++;
			GameManager.instance.onPlayerKilledCallback.Invoke(transform.name, sourcePlayer.name);
		}
		
		
		deaths++;
		
		for (int i = 0; i < disableOnDeath.Length; i++) 
		{
			disableOnDeath[i].enabled = false;
		}

		for (int i = 0; i < disableGameObjectOnDeath.Length; i++) 
		{
			disableGameObjectOnDeath[i].SetActive(false);
		}

		Collider col = GetComponent<Collider>();
		if (col!= null)
		{
			col.enabled = false;
		}
		//Effects
		GameObject _gfxIns = Instantiate(deathEffects, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);
		
		if (isLocalPlayer)
		{
			GameManager.instance.SetSceneCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
		}
		Debug.Log(transform.name + " a été éliminé.");
		StartCoroutine(Respawn());
	}
	
	public float GetHealthPct()
	{
		return (float)currentHealth / maxHealth;
	}
}
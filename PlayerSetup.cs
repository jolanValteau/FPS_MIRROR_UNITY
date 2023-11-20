using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerControlleur))]
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
	[SerializeField]
	Behaviour[] componentsToDisable;
	[SerializeField]
	private string remoteLayerName = "RemotePlayer";
	[SerializeField]
	private string dontDrawLayerName = "DontDraw";
	[SerializeField]
	private GameObject playerGraphics;
	[SerializeField]
	private GameObject playerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance;
	
	private void Start()
	{
		if(!isLocalPlayer)
		{
			DisableComponents();
			AssignRemoteLayer();
		}
		else
		{
			//Disable,ing.Armor
			Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
			
			//Create.UI
			playerUIInstance = Instantiate(playerUIPrefab);
			//Config.UI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if(ui == null)
			{
				Debug.LogError("Pas de PlayerUI sur PlayerUIInstance");
			}
			else
			{
				ui.SetPlayer(GetComponent<Player>());
			}
			GetComponent<Player>().Setup();
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		string netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player player = GetComponent<Player>();
		
		GameManager.RegisterPlayer(netID, player);
	}
	
	private void DisableComponents()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
		{
			componentsToDisable[i].enabled = false;
		}
	}
	
	private void AssignRemoteLayer()
	{
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}
	
	private void OnDisable()
	{
		Destroy(playerUIInstance);
		
		if(isLocalPlayer)
		{
			GameManager.instance.SetSceneCameraActive(true);
			GameManager.UnregisterPlayer(transform.name);
		}
	}
	
}

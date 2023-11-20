using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
	[SerializeField]
	private Camera cam;
	
	[SerializeField]
	private LayerMask mask;
	
	private WeaponManager weaponManager;
	private WeaponData currentWeapon;
	
    void Start()
    {
    	if(cam == null)
    	{
    		Debug.LogError("Pas de caméra renseignée sur le système de tir.");
    			this.enabled = false;
    	}
    	
    	weaponManager = GetComponent<WeaponManager>();
    }
    
    private void Update()
    {
    	currentWeapon = weaponManager.GetCurrentWeapon();
    	
    	if(PauseMenu.isOn)
    	{
    		return;
    	}
    	
    	if(Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
    	{
    		StartCoroutine(weaponManager.Reload());
    		return;
    	}
    	
    	//Test.if.weapon.auto
    	if(currentWeapon.fireRate <= 0f)
    	{
    		if(Input.GetButtonDown("Fire1"))
    		{
    			Shoot();
    		}
    	}
    	else
    	{
    		//Repeat.shoot+cancel///with.mouse
    		if(Input.GetButtonDown("Fire1"))
    		{
    			InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
    		}
    		else if (Input.GetButtonUp("Fire1"))
    		{
    			CancelInvoke("Shoot");
    		}
    	}
    }
    
    //Function,srv.(srv.know.hit)
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
    	RpcDoHitEffect(pos, normal);
    }
    //Spawn.Effects'hit,client.all
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
    	GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
    	Destroy(hitEffect, 2f);
    }
    
    //Function,srv.(srv.know.shoot)
    [Command]
    void CmdOnShoot()
    {
    	RpcDoShootEffect();
    }
    
    //Spawn.Effects'shoot,client.all
    [ClientRpc]
    void RpcDoShootEffect()
    {
    	weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
    
    [Client]
    private void Shoot()
    {
    	if(!isLocalPlayer || weaponManager.isReloading)
    	{
    		return;
    	}
    	if(weaponManager.currentMagazineSize <= 0)
    	{
    		StartCoroutine(weaponManager.Reload());
    		return;
    	}
    	
    	weaponManager.currentMagazineSize--;
    	
    	CmdOnShoot();
    	RaycastHit hit;
    	
    	if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
    	{
    		if(hit.collider.tag == "Player")
    		{
    			CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
    		}
    		
    		CmdOnHit(hit.point, hit.normal);
    	}
    }
    
    [Command]
    private void CmdPlayerShot(string playerId, float damage,string sourceID)
    {
    	Debug.Log(playerId + " a été touché.");
    	
    	Player player = GameManager.GetPlayer(playerId);
    	player.RpcTakeDamage(damage, sourceID);
    }

}

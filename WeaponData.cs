using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "My Game motherfucker/Weapon Data")]
public class WeaponData : ScriptableObject
{
	public string name = "currentGun";
	public float damage = 5f;
	public float range = 100f;
	
	public float fireRate = 10f;
	
	public int magazineSize = 10;
	public float reloadTime = 1.5f;
	
	public GameObject graphics;
}

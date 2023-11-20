using UnityEngine;

public class PlayerUI : MonoBehaviour
{

	[SerializeField]
	private RectTransform thrusterFuelFill;


	[SerializeField]
	private RectTransform healthBarFill;

	private PlayerControlleur controlleur;
	private Player player;
	private WeaponManager weaponManager;

	[SerializeField]
	private GameObject pauseMenu;

	[SerializeField]
	private GameObject scoreboard;



	public void SetPlayer(Player _player)
	{
		player = _player;
		controlleur = player.GetComponent<PlayerControlleur>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

	private void Start()
	{
		PauseMenu.isOn = false;
	}




	private void Update()
	{

		if (player == null)
		{
			Debug.Log("NUHBHJKGFBHGFCGVB JG?");
		}
		else
		{
			SetHealthAmount(player.GetHealthPct());
			SetFuelAmount(controlleur.GetThrusterFuelAmount());
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			scoreboard.SetActive(true);
		}
		else if (Input.GetKeyUp(KeyCode.Tab))
		{
			scoreboard.SetActive(false);
		}
	}

	public void TogglePauseMenu()
	{
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.isOn = pauseMenu.activeSelf;
	}

	public void SetFuelAmount(float _amount)
	{
		thrusterFuelFill.localScale = new Vector3(_amount, 1f, 1f);
	}

	public void SetHealthAmount(float _amount)
	{
        healthBarFill.localScale = new Vector3(_amount, 1f, 1f);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
	
	
	[SerializeField]
	GameObject playerScoreboardItem;
	
	[SerializeField]
	Transform playerScoreboardList;
	
	private void OnEnable()
	{
		//Get.array'all,player
		Player[] players = GameManager.GetAllPlayers();
		
		//loop.maj.list
		foreach (Player player in players)
		{
			GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
			playerScoreboardItem item = itemGO.GetComponent<playerScoreboardItem>();
			if(item != null)
			{
				item.Setup(player);
			}
		}
		
	}
	
	private void OnDisable()
	{
		//clear
		foreach (Transform child in playerScoreboardList)
		{
			Destroy(child.gameObject);
		}
	}
}

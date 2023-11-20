using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
	private const string playerIDPrefix = "Player";
	
	private static Dictionary<string, Player> players = new Dictionary<string, Player>();
	
	public MatchSettings matchSettings;
	
	public static GameManager instance;
	
	[SerializeField]
	private GameObject sceneCamera;
	
	public delegate void OnPlayerKilledCallback(string player, string source);
	public OnPlayerKilledCallback onPlayerKilledCallback;
	
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		
		Debug.LogError("Plus d'une instance de GameManager dans la scène");
	}
	
	public void SetSceneCameraActive(bool isActive)
	{
		if(sceneCamera == null)
		{
			return;
		}
		
		sceneCamera.SetActive(isActive);
	}
	public static void RegisterPlayer(string netID, Player player)
	{
		string playerID = playerIDPrefix + netID;
		players.Add(playerID, player);
		player.transform.name = playerID;
	}
	
	public static void UnregisterPlayer(string playerID)
	{
		players.Remove(playerID);
	}
	
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(200, 100, 200, 500));
		GUILayout.BeginVertical();
		foreach(string playerID in players.Keys)
		{
			GUILayout.Label(playerID + " - " + players[playerID].transform.name);
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public static Player GetPlayer(string playerID)
	{
		return players[playerID];
	}
	
	public static Player[] GetAllPlayers()
	{
		return players.Values.ToArray();
	}
}
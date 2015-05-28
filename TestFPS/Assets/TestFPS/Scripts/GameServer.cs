using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameServer : MonoBehaviour
{
	public GameObject FPSPlayerPrefab;

	private Dictionary<NetworkPlayer, GameObject> m_Players;
	private int m_num_players;

	
	public GameObject m_ServerCamera;
	
	// Use this for initialization
	void Start ()
	{
		m_Players = new Dictionary<NetworkPlayer, GameObject> ();
		GameObject.Instantiate (m_ServerCamera, Vector3.up * 10 + Vector3.forward * 3 + Vector3.right * 3, Quaternion.Euler (90, 0, 0));
	}
    
	// Update is called once per frame
	void Update ()
	{

	}

	void OnPlayerConnected (NetworkPlayer player)
	{
		Debug.Log ("player connected");
		m_Players.Add (player, SpawnPlayer ());
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		DeSpawnPlayer (player);
	}

	public GameObject GetPlayer (NetworkPlayer player)
	{
		return m_Players [player];
	}
	
	private GameObject SpawnPlayer ()
	{
		Debug.Log ("player spawned");
		return Network.Instantiate (FPSPlayerPrefab, Vector3.up * 5 + Vector3.forward * 3 + Vector3.right * 3, Quaternion.identity, 0) as GameObject;
	}
	
	private void DeSpawnPlayer (NetworkPlayer player)
	{
		Debug.Log ("player Despawned");
		Network.Destroy (m_Players [player]);
		m_Players.Remove (player);
	}
}

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	private const string typeName = "Hush_Game_V0.1";
	private const string gameName = "RoomName";

	private bool isRefreshingHostList = false;
	private HostData[] hostList;

	public GameObject FPSController;
	public GameObject GameServer;

	void OnGUI ()
	{
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server"))
				StartServer ();

			if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList ();

			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName))
						JoinServer (hostList [i]);
				}
			}
		}
		if (Network.isServer) {
			GUI.Label (new Rect (100, 100, 250, 100), "I am a server!");
		}
	}

	private void StartServer ()
	{
		Network.InitializeServer (5, 25000, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (typeName, gameName);
		GameObject.Instantiate (GameServer, Vector3.up * 5, Quaternion.identity);
	}

	void OnServerInitialized ()
	{
		Debug.Log ("Server initialzied");
	}


	void Update ()
	{
		if (isRefreshingHostList && MasterServer.PollHostList ().Length > 0) {
			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList ();
		}
	}

	private void RefreshHostList ()
	{
		if (!isRefreshingHostList) {
			isRefreshingHostList = true;
			MasterServer.RequestHostList (typeName);
		}
	}

	private void JoinServer (HostData hostData)
	{
		Network.Connect (hostData);
	}


	void OnConnectedToServer ()
	{
		SpawnPlayer ();
	}

	private void SpawnPlayer ()
	{
		GameObject.Instantiate (FPSController, Vector3.up * 5 + Vector3.forward * 3 + Vector3.right * 8, Quaternion.identity);
	}
}

using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.ScoreboardPhoton
{
	public class GameManager : MonoBehaviourPunCallbacks
	{
		public static GameManager instance;

		private bool TeamScene = false;
		private bool host = false;
		private ShowStatus status;

		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			status = GetComponent<ShowStatus>();
			UIGlobalPUN.instance.InputLogin.text = "Player_" + Random.Range(1000, 10000);
		}

		public void OnLogin(string pseudo, bool ishost, bool team)
		{
			TeamScene = team;
			host = ishost;
			UIGlobalPUN.instance.panelLogin.SetActive(false);
			PhotonNetwork.LocalPlayer.NickName = pseudo;
			PhotonNetwork.ConnectUsingSettings();
			status.enabled = true;
		}

		public override void OnConnectedToMaster()
		{
			if (host)
			{
				CreateRoom();
			}
			else
			{
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnJoinedRoom()
		{
			status.enabled = false;
		}

		public void OnPhotonRandomJoinFailed()
		{ 
            CreateRoom();
		}

		public void CreateRoom()
		{
			string roomName = "Table_" + Random.Range(1000, 10000);
			roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
			RoomOptions options = new RoomOptions { MaxPlayers = 10 };
			options.CustomRoomProperties = new Hashtable() {{ "teamGame", TeamScene ? "yes" : "no"}};
			PhotonNetwork.CreateRoom(roomName, options, null);
		}
	}
}
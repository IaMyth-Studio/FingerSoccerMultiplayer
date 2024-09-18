using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.ScoreboardPhoton
{
	public class PlayerNumber : MonoBehaviourPunCallbacks
	{

		#region Public Properties

		public static PlayerNumber instance;

		public static Player[] SortedPlayers;
		public delegate void PlayerNumberChanged();
		public static event PlayerNumberChanged OnPlayerNumberChanged;

		public const string RoomPlayerIndexedProp = "pNr";
		public bool dontDestroyOnLoad = false;


		#endregion


		#region MonoBehaviours methods

		public void Awake()
		{

			if (instance != null && instance != this && instance.gameObject != null)
			{
				DestroyImmediate(instance.gameObject);
			}

			instance = this;
			if (dontDestroyOnLoad)
			{
				DontDestroyOnLoad(this.gameObject);
			}

			this.RefreshData();
		}

		#endregion


		#region PunBehavior Overrides

		public override void OnJoinedRoom()
		{
			this.RefreshData();
		}

		public override void OnLeftRoom()
		{
			PhotonNetwork.LocalPlayer.CustomProperties.Remove(PlayerNumber.RoomPlayerIndexedProp);
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			this.RefreshData();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			this.RefreshData();
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (changedProps != null && changedProps.ContainsKey(PlayerNumber.RoomPlayerIndexedProp))
			{
				this.RefreshData();
			}
		}

		#endregion

		public void RefreshData()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return;
			}

			if (PhotonNetwork.LocalPlayer.GetPlayerNumber() >= 0)
			{
				SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();
				if (OnPlayerNumberChanged != null)
				{
					OnPlayerNumberChanged();
				}
				return;
			}


			HashSet<int> usedInts = new HashSet<int>();
			Player[] sorted = PhotonNetwork.PlayerList.OrderBy((p) => p.ActorNumber).ToArray();

			string allPlayers = "all players: ";
			foreach (Player player in sorted)
			{
				allPlayers += player.ActorNumber + "=pNr:" + player.GetPlayerNumber() + ", ";

				int number = player.GetPlayerNumber();

				if (player.IsLocal)
				{
					Debug.Log("PhotonNetwork.CurrentRoom.PlayerCount = " + PhotonNetwork.CurrentRoom.PlayerCount);

					for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
					{
						if (!usedInts.Contains(i))
						{
							player.SetPlayerNumber(i);
							break;
						}
					}
					break;
				}
				else
				{
					if (number < 0)
					{
						break;
					}
					else
					{
						usedInts.Add(number);
					}
				}
			}

			SortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy((p) => p.GetPlayerNumber()).ToArray();
			if (OnPlayerNumberChanged != null)
			{
				OnPlayerNumberChanged();
			}
		}
	}



	public static class PlayerNumberExtensions
	{
		public static int GetPlayerNumber(this Player player)
		{
			if (player == null)
			{
				return -1;
			}

			if (PhotonNetwork.OfflineMode)
			{
				return 0;
			}
			if (!PhotonNetwork.IsConnectedAndReady)
			{
				return -1;
			}

			object value;
			if (player.CustomProperties.TryGetValue(PlayerNumber.RoomPlayerIndexedProp, out value))
			{
				return (byte)value;
			}
			return -1;
		}

		public static void SetPlayerNumber(this Player player, int playerNumber)
		{
			if (player == null)
			{
				return;
			}

			if (PhotonNetwork.OfflineMode)
			{
				return;
			}

			if (playerNumber < 0)
			{
				Debug.LogWarning("Setting invalid playerNumber: " + playerNumber + " for: " + player.ToStringFull());
			}

			if (!PhotonNetwork.IsConnectedAndReady)
			{
				Debug.LogWarning("SetPlayerNumber was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
				return;
			}

			int current = player.GetPlayerNumber();
			if (current != playerNumber)
			{
				Debug.Log("PlayerNumber: Set number " + playerNumber);
				player.SetCustomProperties(new Hashtable() { { PlayerNumber.RoomPlayerIndexedProp, (byte)playerNumber } });
			}
		}
	}
}
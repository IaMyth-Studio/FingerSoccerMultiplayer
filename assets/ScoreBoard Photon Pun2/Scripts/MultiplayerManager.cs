using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.ScoreboardPhoton
{
	public class MultiplayerManager : MonoBehaviourPunCallbacks
	{
		public static MultiplayerManager instance;
		public GameObject PlayerListEntryPrefab;
		public Transform InsideScoreSolo;
		public Transform InsideScoreTeamRed;
		public Transform InsideScoreTeamBlue;
		public Text countPlayers;
		public Text countPlayersRedTeam;
		public Text countPlayersBlueTeam;
		public List<MPPlayer> playerListEntries = new List<MPPlayer>();

		private string team;
		private bool IsTeam;
		private string commandTeam;

		void Awake()
		{
			instance = this;
		}

		public override void OnJoinedRoom() // IS LAUNCHED AUTOMATICALLY WHEN JOINING THE ROOM PHOTON.
		{
			if (playerListEntries == null)
			{
				playerListEntries = new List<MPPlayer>(); // CREATE LIST PLAYERENTRY IF NO LIST
			}

			Hashtable props = new Hashtable {{ ScoreBoard.PLAYER_TEAM, team }, { ScoreBoard.PLAYER_KILLS, ScoreBoard.PLAYER_KILL }, //LOOK DOC HELP PHOTON "CUSTOMPROPERTIES"
				{ ScoreBoard.PLAYER_DEATHS, ScoreBoard.PLAYER_DEATH }, { ScoreBoard.PLAYER_RANK, ScoreBoard.PLAYER_LVL }};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);

			commandTeam = PhotonNetwork.CurrentRoom.CustomProperties["teamGame"] as string; //CHECK IN "CUSTOMPROPERTIES" IF CURRENT ROOM IS TEAM GAME OR SOLO GAME
			IsTeam = commandTeam == "yes" ? true : false;

			if (IsTeam) //IF TEAM GAME
			{
				UIGlobalPUN.instance.ChooseTeam.SetActive(true); //ACTIVE "CHOOSE TEAM" PANEL
			}
			else
			{
				Hashtable prop = new Hashtable { { ScoreBoard.PLAYER_TEAM, team } }; // ADJUST YOUR TEAM OR NO TEAM IN CUSTOM PROPERTIES
				PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

				photonView.RPC("AddToScoreBoard", RpcTarget.AllViaServer); // AND LAUNCH AT ALL PLAYER FOR ADD OBJECT IN SCOREBOARD AND IN LIST PLAYER LINE 111
			}
		}

		public override void OnPlayerLeftRoom(Player otherPlayer) // IS LAUNCHED AUTOMATICALLY WHEN PLAYER LEFT ROOM.
		{
			MPPlayer temp = null;
			foreach (MPPlayer entry in playerListEntries.ToArray()) // CHECK IN LIST PLAYER
			{
				if (entry.ActorID == otherPlayer.ActorNumber) // IF ACTOR ID IS EQUAL AT ACTOR ID OF PLAYER WHO QUIT THE ROOM
				{
					temp = entry;
					Destroy(entry.ObjForScorepanel); // SO DELETE GAMEOBJECT IN SCOREBOARD AT PLAYER WHO QUIT THE ROOM
				}
			}

			if (temp != null)
			{
				playerListEntries.Remove(temp);// AND REMOVE IN LIST PLAYER
			}

            AddCountingPlayer(); //REFRESH COUNT TOTAL PLAYERS
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) //Called when custom player-properties are changed
		{
			foreach (MPPlayer player in playerListEntries.ToArray()) // CHECK IN LIST PLAYER
			{
				if (player.ActorID == targetPlayer.ActorNumber) // CHECK THE PLAYER ACTOR ID IN THE LIST WHO CALLED CUSTOM PROPERTIES
				{
					object Playerkill;
					if (changedProps.TryGetValue(ScoreBoard.PLAYER_KILLS, out Playerkill)) // CHECK IF CALLED CUSTOM PROPERTIES IS "KILL"
					{
						player.ObjForScorepanel.GetComponent<InitPlayerEntry>().PlayerKillText.text = string.Format(targetPlayer.IsLocal ? "<color=#000000>{0}</color>" : "{0}", Playerkill); // REFRESH "KILL" IN SCOREBOARD
						player.Kills = (int)Playerkill;// REFRESH "KILL" IN LIST PLAYER
					}

					object PlayerDeath;
					if (changedProps.TryGetValue(ScoreBoard.PLAYER_DEATHS, out PlayerDeath)) // CHECK IF CALLED CUSTOM PROPERTIES IS "DEATH"
					{
						player.ObjForScorepanel.GetComponent<InitPlayerEntry>().PlayerDeathText.text = string.Format(targetPlayer.IsLocal? "<color=#000000>{0}</color>" : "{0}", PlayerDeath); // REFRESH "DEATH" IN SCOREBOARD
						player.Deaths = (int)PlayerDeath;// REFRESH "DEATH" IN LIST PLAYER
					}
					object PlayerLvl;
					if (changedProps.TryGetValue(ScoreBoard.PLAYER_RANK, out PlayerLvl)) // CHECK IF CALLED CUSTOM PROPERTIES IS "RANK"
					{
						player.ObjForScorepanel.GetComponent<InitPlayerEntry>().PlayerRankText.text = string.Format("{0}", PlayerLvl);
						player.ObjForScorepanel.GetComponent<InitPlayerEntry>().ImageRank.sprite = player.ObjForScorepanel.GetComponent<InitPlayerEntry>().imgRank[(int)PlayerLvl]; // REFRESH "RANK" IN SCOREBOARD
						player.level = (int)PlayerLvl; // REFRESH "RANK" IN LIST PLAYER
					}

					player.ObjForScorepanel.GetComponent<Image>().sprite = targetPlayer.IsLocal ? player.ObjForScorepanel.GetComponent<InitPlayerEntry>().imgBg[0] : player.ObjForScorepanel.GetComponent<InitPlayerEntry>().imgBg[1]; // ADJUST BACKGROUND OBJECT IN SCOREBOARD FOR LOCAL PLAYER
				}
			}
		}

		[PunRPC]
		public void AddToScoreBoard() // IS LAUNCHED AFTER SELECT TEAM OR NO TEAM FOR ADD OBJECT IN SCOREBOARD
		{
			foreach (MPPlayer entry in playerListEntries)
			{
				Destroy(entry.ObjForScorepanel); // DESTROY ALL OBJECT IN SCOREBOARD FOR REFRESH
			}

			playerListEntries.Clear();// DESTROY ALL PLAYERS IN LIST FOR REFRESH

			foreach (Player p in PhotonNetwork.PlayerList.ToArray()) // CREATE NEW LIST PLAYERS AND NEW OBJECT IN SCOREBOARD
			{
				GameObject entry = Instantiate(PlayerListEntryPrefab); // CREATE NEW OBJECT
				if (IsTeam) // IF TEAM GAME
				{
					UIGlobalPUN.instance.ScoreBoardPanelTeam.SetActive(true); // ACTIVE TEAM SCOREBOARD PANEL
					if ((string)p.CustomProperties[ScoreBoard.PLAYER_TEAM] == "red") // IF TEAM OF PLAYER IN "PhotonNetwork.PlayerList" IS "RED"
					{
						entry.transform.SetParent(InsideScoreTeamRed.transform); // ADJUST PARENT OF OBJECT IN CORRECTLY TEAM
					}
					else if ((string)p.CustomProperties[ScoreBoard.PLAYER_TEAM] == "blue") // IF TEAM OF PLAYER IN "PhotonNetwork.PlayerList" IS "BLUE"
					{
						entry.transform.SetParent(InsideScoreTeamBlue.transform); // ADJUST PARENT OF OBJECT IN CORRECTLY TEAM
					}
				}
				else // IF NO TEAM GAME
				{
					UIGlobalPUN.instance.ScoreBoardPanelSolo.SetActive(true); // ACTIVE SOLO SCOREBOARD PANEL
					entry.transform.SetParent(InsideScoreSolo.transform); // ADJUST PARENT OF OBJECT IN SCOREBOARD
				}

				entry.transform.localScale = Vector3.one; // RESIZE SCALE
				entry.GetComponent<InitPlayerEntry>().Initialize(p.ActorNumber, p.NickName); //ADD IN OBJECT ACTOR ID AND PSEUDO OF PLAYER
				entry.GetComponent<InitPlayerEntry>().PlayerNameText.text = string.Format(p.IsLocal ? "<color=#000000>{0}</color>" : "{0}", p.NickName); //ADD IN OBJECT TEXT THE PSEUDO OF PLAYER AND CHANGE COLOR IF IS LOCAL PLAYER
				entry.GetComponent<InitPlayerEntry>().PlayerRankText.text = string.Format("{0}", p.CustomProperties[ScoreBoard.PLAYER_RANK]); //ADD IN OBJECT TEXT THE LEVEL OF PLAYER
				entry.GetComponent<InitPlayerEntry>().PlayerDeathText.text = string.Format(p.IsLocal ? "<color=#000000>{0}</color>" : "{0}", p.CustomProperties[ScoreBoard.PLAYER_DEATHS]); //ADD IN OBJECT TEXT THE DEATHS OF PLAYER AND CHANGE COLOR IF IS LOCAL PLAYER
				entry.GetComponent<InitPlayerEntry>().PlayerKillText.text = string.Format(p.IsLocal ? "<color=#000000>{0}</color>" : "{0}", p.CustomProperties[ScoreBoard.PLAYER_KILLS]); //ADD IN OBJECT TEXT THE KILLS OF PLAYER AND CHANGE COLOR IF IS LOCAL PLAYER
				entry.GetComponent<InitPlayerEntry>().ImageRank.sprite = entry.GetComponent<InitPlayerEntry>().imgRank[(int)p.CustomProperties[ScoreBoard.PLAYER_RANK]]; // ADJUST IMAGE RANK IN OBJECT IN FUCNTION OF LEVEL
				entry.GetComponent<Image>().sprite = p.IsLocal ? entry.GetComponent<InitPlayerEntry>().imgBg[0] : entry.GetComponent<InitPlayerEntry>().imgBg[1]; // ADJUST BACKGROUND IF IS LOCAL PLAYER

				MPPlayer ply = new MPPlayer(); // ADD PLAYER TO LIST
				ply.pseudo = p.NickName; // ADD PSEUDO
				ply.ObjForScorepanel = entry; // ADD OBJECT
				ply.level = (int)p.CustomProperties[ScoreBoard.PLAYER_RANK]; // ADD LEVEL
				ply.Kills = (int)p.CustomProperties[ScoreBoard.PLAYER_KILLS]; // ADD KILLS
				ply.Deaths = (int)p.CustomProperties[ScoreBoard.PLAYER_DEATHS]; // ADD DEATHS
				ply.team = (string)p.CustomProperties[ScoreBoard.PLAYER_TEAM]; // ADD TEAM IF TEAM
				ply.ActorID = p.ActorNumber; // ADD ACTOR ID

				playerListEntries.Add(ply); // ADD TO LIST
				AddCountingPlayer(); // ADJUST COUNT TOTAL PLAYER
			}
		}

		[PunRPC]
		public void SortGameObject() // SORT OBJECT IN SCOREBOARD IN FUCNTION OF NUMBERS KILLS "OrderByDescending". LOOK IN "InitPlayerEntry.cs" ON BUTTON "ADD KILLS", "ADD DEATHS" AND "ADD LEVEL"
		{							 // IN HIERARCHY TO "CANVAS/ScoreBoard_Panel" OR TO "CANVAS/ScoreBoard_Panel_Team" in "Bottom_bar" CHILDS.
			foreach (MPPlayer player in playerListEntries.OrderByDescending(p => p.Kills).ToArray())
			{
				player.ObjForScorepanel.GetComponent<RectTransform>().SetAsLastSibling();
			}
		}

		public void OnClickBtn(string id) // FUNCTION AFTER "CLICK" BUTTON "ADD KILL" "ADD DEATH" "ADD LEVEL" IN HIERARCHY TO "CANVAS/ScoreBoard_Panel" OR TO "CANVAS/ScoreBoard_Panel_Team" in "Bottom_bar" CHILDS.
		{
			foreach (MPPlayer entry in playerListEntries) // CHECK ALL PLAYER IN LIST
			{
				if (entry.ActorID == PhotonNetwork.LocalPlayer.ActorNumber) // IF ACTOR ID IS EQUAL TO ACTOR ID OF LOCAL PLAYER
				{
					if (id == "AddKill")
					{
						entry.Kills += 1;
					}
					else if (id == "AddDeath")
					{
						entry.Deaths += 1;
					}
					else if (id == "AddLevel")
					{
						entry.level += 1;
					}
					entry.ObjForScorepanel.GetComponent<InitPlayerEntry>().ClickAdd(id); // AND SEND ACTION TO "InitPlayerEntry" IN OBJECT. (OBJECT IS ON "ASSET/RESOURCES/PlayerScore").
				}
			}
		}

		public void TeamChoose(string tea_m) // FUNCTION AFTER "CLICK" BUTTON TEAM IN HIERARCHY TO "CANVAS/ChooseTeam" PANEL. LOOK CHILD
		{
			team = tea_m;
			UIGlobalPUN.instance.ChooseTeam.SetActive(false); // DESACTIVE "CHOOSE TEAM" PANEL

			Hashtable props = new Hashtable {{ScoreBoard.PLAYER_TEAM, team}}; // ADD YOUR TEAM IN CUSTOM PROPERTIES
			PhotonNetwork.LocalPlayer.SetCustomProperties(props); // ADD YOUR TEAM IN CUSTOM PROPERTIES

			photonView.RPC("AddToScoreBoard", RpcTarget.AllViaServer); // LAUNCH AT ALL PLAYER FOR ADD OBJECT IN SCOREBOARD AND IN LIST PLAYER LINE 111
		}


		private int CountPlayers()
		{
			return PhotonNetwork.CurrentRoom.PlayerCount; // RETURN COUNT PLAYER IN ROOM PHOTON
		}

		private int CountPlayersTeam(string idTeam) // RETURN COUNT PLAYER IN LIST PLAYER WITH TEAM
		{
			int count = 0;
			foreach (MPPlayer p in playerListEntries)
			{
				if (p.team == idTeam) // IF TEAM PLAYER IS EQUAL AT "idTeam"
				{
					count++;
				}
			}
			return count;
		}

		private void AddCountingPlayer() // ADJUST TEXT ALL COUNT PLAYER IN TEXT
		{
			countPlayersRedTeam.text = "[<color=#ffa500ff>" + CountPlayersTeam("red").ToString("0") + "</color>]";
			countPlayersBlueTeam.text = "[<color=#ffa500ff>" + CountPlayersTeam("blue").ToString("0") + "</color>]";
			countPlayers.text = CountPlayers().ToString("0");
		}
	}
}


namespace Photon.Pun.ScoreboardPhoton
{
	[System.Serializable]
	public class MPPlayer
	{
		public string pseudo;
		public GameObject ObjForScorepanel;
		public int level = 0;
		public int Deaths = 0;
		public int Kills = 0;
		public string team;
		public int ActorID;
	}
}

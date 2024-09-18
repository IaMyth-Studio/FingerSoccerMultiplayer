using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.ScoreboardPhoton
{
	public class InitPlayerEntry : MonoBehaviour
	{
		[Header("UI References")]
		public Text PlayerNameText;
		public Text PlayerKillText;
		public Text PlayerDeathText;
		public Text PlayerRankText;
		public Image ImageRank;
		public Sprite[] imgRank;
		public Sprite[] imgBg;

		private int ownerId;

		public void Initialize(int playerId, string playerName)
		{
			ownerId = playerId;
			PlayerNameText.text = playerName;
		}

		public void ClickAdd(string id)
		{
			if (id == "AddKill")
			{
				object kills;
				if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ScoreBoard.PLAYER_KILLS, out kills))
				{
					PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { ScoreBoard.PLAYER_KILLS, ((int)kills + 1) } });
				}
			}
			else if (id == "AddDeath")
			{
				object deaths;
				if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ScoreBoard.PLAYER_DEATHS, out deaths))
				{
					PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { ScoreBoard.PLAYER_DEATHS, ((int)deaths + 1) } });
				}
			}
			else if (id == "AddLevel")
			{
				object ranks;
				if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ScoreBoard.PLAYER_RANK, out ranks))
				{
					PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { ScoreBoard.PLAYER_RANK, ((int)ranks >= imgRank.Length-1 ? imgRank.Length-1 : ((int)ranks + 1)) } });
				}
			}
			MultiplayerManager.instance.photonView.RPC("SortGameObject", RpcTarget.AllViaServer);
		}
	}
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Photon.Pun.ScoreboardPhoton
{
	public class UIGlobalPUN : MonoBehaviour
	{
		public static UIGlobalPUN instance;
		public GameObject panelLogin;
		public TMP_InputField InputLogin;
		public GameObject BtnCreateGame;
		public GameObject BtnJoinGame;
		public GameObject ChooseTeam;
		public GameObject ScoreBoardPanelSolo;
		public GameObject ScoreBoardPanelTeam;

		void Awake()
		{
			instance = this;
		}
	}
}
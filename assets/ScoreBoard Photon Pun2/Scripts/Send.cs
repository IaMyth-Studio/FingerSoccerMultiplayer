using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.ScoreboardPhoton
{
	public class Send : MonoBehaviour
	{
		public void Login(string id)
		{
			if (!UIGlobalPUN.instance.InputLogin.text.Equals("")) //IF INPUTFIELD FOR LOGIN IS NOT EQUAL AT EMPTY
			{
				string login = UIGlobalPUN.instance.InputLogin.text;
				if (id == "create")
				{
					GameManager.instance.OnLogin(login, true, false); //FIRST "TRUE" YOU CONFIRM IS "CREATE ROOM" AND NEXT "FALSE" YOU CONFIRM IS NOT CREATE TEAM GAME TO FUNCTION "OnLogin()" IN "GAMEMANAGER.CS", LINE 26.
				}
				else if (id == "createTeam")
				{
					GameManager.instance.OnLogin(login, true, true); //FIRST "TRUE" YOU CONFIRM IS "CREATE ROOM" AND NEXT "TRUE" YOU CONFIRM IS CREATE TEAM GAME TO FUNCTION "OnLogin()" IN "GAMEMANAGER.CS", LINE 26.
				}
				else if (id == "join")
				{
					GameManager.instance.OnLogin(login, false, false); //FIRST "FALSE" YOU CONFIRM IS NOT "CREATE ROOM", YOU JOIN ROOM TO FUNCTION "OnLogin()" IN "GAMEMANAGER.CS", LINE 26.
				}
			}
		}
	}
}
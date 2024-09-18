using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class LoadPHP : MonoBehaviour
{
	public static LoadPHP instance;
	public Register register;
	char[] splitchar = { '|' };
	string login_variables_url = "https://iamyth.sytes.net/GAMES/Php_Game_FIles/V1.0/login.php";
	string load_variables_url = "https://iamyth.sytes.net/GAMES/Php_Game_FIles/V1.0/loadStats.php";
	string load_login_url = "https://iamyth.sytes.net/GAMES/Php_Game_FIles/V1.0/VerifLogin.php";
	string register_url = "https://iamyth.sytes.net/GAMES/Php_Game_FIles/V1.0/register.php";

	bool sendCurrent;

	void Awake()
	{
		instance = this;
	}

	public void phpLogin()//SEND TO PHP IN STARTCOROUTINE FOR LOGIN
	{
		if (!sendCurrent)
		{
			UIGlobal.instance.AllText[0].text = "";
			StartCoroutine("Login");
		}
	}

	IEnumerator Login() //SEND AND LOAD TO PHP FOR LOGIN
	{
		// Asegurarse de que UIGlobal.instance no es null
		if (UIGlobal.instance == null)
		{
			Debug.LogError("UIGlobal.instance is null");
			yield break;
		}

		// Asegurarse de que los campos MyName y MyPass no son null
		if (string.IsNullOrEmpty(UIGlobal.instance.MyName) || string.IsNullOrEmpty(UIGlobal.instance.MyPass))
		{
			Debug.LogError("MyName or MyPass is null or empty");
			yield break;
		}

		sendCurrent = true;
		WWWForm form = new WWWForm();
		form.AddField("playerName", UIGlobal.instance.MyName);
		form.AddField("password", UIGlobal.instance.MyPass);
		form.AddField("key", "9f28f10d96d7227ba213d613a38fb5be");
		var download = UnityWebRequest.Post(login_variables_url, form);

		yield return download.SendWebRequest();

		if (download.isNetworkError || download.isHttpError)
		{
			Debug.LogError("Network error: " + download.error);
		}
		else
		{
			// Comprobamos que AccessDenied y AccessGranted no son null antes de usarlos
			if (UIGlobal.instance.AccessDenied == null || UIGlobal.instance.AccessGranted == null)
			{
				Debug.LogError("AccessDenied or AccessGranted is null");
				yield break;
			}

			if (download.downloadHandler.text == "Error login" || download.downloadHandler.text == "Bad login")
			{
				UIGlobal.instance.AccessDenied.SetActive(true);
				UIGlobal.instance.AccessGranted.SetActive(false);
				sendCurrent = false;
			}
			else if (download.downloadHandler.text == "Login success")
			{
				UIGlobal.instance.AccessGranted.SetActive(true);
				UIGlobal.instance.AccessDenied.SetActive(false);

				// Asegurarse de que PlayerData.instance no es null antes de asignar valores
				if (PlayerData.instance != null)
				{
					PlayerData.instance.playerName = UIGlobal.instance.MyName;
					PlayerData.instance.playerEmail = UIGlobal.instance.MyEmail;
				}
				else
				{
					Debug.LogError("PlayerData.instance is null");
				}

				// Continuar con la carga de estadísticas si todo está bien
				StartCoroutine("LoadStats");
			}
		}

		sendCurrent = false;
	}


	IEnumerator LoadStats()//LOAD TO PHP THE NAME, PASSWORD, MONEYS, ETC...IN MYSQL WITH YOUR LOGIN
	{
		WWWForm form = new WWWForm();
		form.AddField("playerName", UIGlobal.instance.MyName);
		form.AddField("password", UIGlobal.instance.MyPass);
		form.AddField("key", "9f28f10d96d7227ba213d613a38fb5be");
		var download = UnityWebRequest.Post(load_variables_url, form);

		yield return download.SendWebRequest();
		if (download.isNetworkError || download.isHttpError)
		{

		}
		else
		{
			if (download.downloadHandler.text == "Error login" || download.downloadHandler.text == "Bad login")
			{
				UIGlobal.instance.AccessDenied.SetActive(true);
				UIGlobal.instance.AccessGranted.SetActive(false);
				sendCurrent = false;
				StopCoroutine("LoadStats");
			}
			else
			{
			    string[] variables = download.downloadHandler.text.Split(splitchar);
			    PlayerData.instance.playerLevel = int.Parse(variables[0]);
			    PlayerData.instance.playerEmail = variables[1];
			    PlayerData.instance.playerMoneys = int.Parse(variables[2]);
			    PlayerData.instance.avatarID = int.Parse(variables[3]);
			
			    UIGlobal.instance.Level = PlayerData.instance.playerLevel;
			    UIGlobal.instance.MyEmail = PlayerData.instance.playerEmail;
			    UIGlobal.instance.Moneys = PlayerData.instance.playerMoneys;
			    UIGlobal.instance.AvatarID = PlayerData.instance.avatarID;
			
			    UIGlobal.instance.AvatarImageConnexion.sprite = UIGlobal.instance.AllAvatar[UIGlobal.instance.AvatarID];
			    UIGlobal.instance.AvatarImageMenu.sprite = UIGlobal.instance.AllAvatar[UIGlobal.instance.AvatarID];
			
			    sendCurrent = false;
			    StopCoroutine("LoadStats");
			}
		}
	}

	IEnumerator VerifLogin()//CHECK IN PHP IF LOGIN IS ALREADY USED WHEN CREATE NEW ACCOUNT
	{
		sendCurrent = true;
		WWWForm form = new WWWForm();
		form.AddField("playerName", UIGlobal.instance.AllInputtext[2].text);
		form.AddField("key", "9f28f10d96d7227ba213d613a38fb5be");
		var download = UnityWebRequest.Post(load_login_url, form);

		yield return download.SendWebRequest();
		if (download.isNetworkError || download.isHttpError)
		{
			// Manejo de errores
		}
		else
		{
			if (download.downloadHandler.text == "Error login" || download.downloadHandler.text == "Bad login")
			{
				if (!string.IsNullOrEmpty(UIGlobal.instance.AllInputtext[2].text))
				{
					UIGlobal.instance.AllText[13].color = Color.red;
					UIGlobal.instance.AllText[13].text = "Already used!";
					register.LoginValid = false;
				}
				else
				{
					UIGlobal.instance.AllText[13].text = "";
					register.LoginValid = false;
				}
				sendCurrent = false;
				StopCoroutine("VerifLogin");
			}
			else if (download.downloadHandler.text == "Login success")
			{
				if (!string.IsNullOrEmpty(UIGlobal.instance.AllInputtext[2].text))
				{
					UIGlobal.instance.AllText[13].color = Color.green;
					UIGlobal.instance.AllText[13].text = "Available!";
					UIGlobal.instance.MyName = UIGlobal.instance.AllInputtext[2].text;
					register.LoginValid = true;

					// Guardar datos en PlayerData
					PlayerData playerData = PlayerData.instance;
					playerData.playerName = UIGlobal.instance.MyName;

					DontDestroyOnLoad(playerData.gameObject); // Asegurarse de que PlayerData persista entre escenas
				}
				else
				{
					UIGlobal.instance.AllText[13].text = "";
					register.LoginValid = false;
				}
				sendCurrent = false;
				StopCoroutine("VerifLogin");
			}
		}
	}


	IEnumerator NewAccount()//SEND TO PHP FOR CREATE NEW ACCOUNT IN MYSQL
	{
		sendCurrent = true;
		WWWForm form = new WWWForm();
		form.AddField("playerName", UIGlobal.instance.MyName);
		form.AddField("password", UIGlobal.instance.MyPass);
		form.AddField("email", UIGlobal.instance.MyEmail);
		form.AddField("idavatar", UIGlobal.instance.AvatarID);
		form.AddField("key", "9f28f10d96d7227ba213d613a38fb5be");
		var download = UnityWebRequest.Post(register_url, form);

		yield return download.SendWebRequest();
		if (download.isNetworkError || download.isHttpError)
		{

		}
		else
		{
			if (download.downloadHandler.text == "Error")
			{
				UIGlobal.instance.Registered = true;
				UIGlobal.instance.ErrorRegistered = true;
				UIGlobal.instance.LeaveRegisterPanel();

				sendCurrent = false;
				StopCoroutine("NewAccount");
			}
			else if (download.downloadHandler.text == "Ok")
			{
				UIGlobal.instance.Registered = true;
				UIGlobal.instance.ErrorRegistered = false;
				UIGlobal.instance.LeaveRegisterPanel();

				sendCurrent = false;
				StopCoroutine("NewAccount");
			}
		}
	}
}

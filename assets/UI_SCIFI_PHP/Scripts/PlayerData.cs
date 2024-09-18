using UnityEngine;

public class PlayerData : MonoBehaviour
{
	public static PlayerData instance;

	public string playerName;
	public string playerEmail;
	public int playerLevel;
	public int playerMoneys;
	public int avatarID;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}


using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class SavePlayerProgressOnline : NetworkBehaviour 
{
	PlayerAchievementCount m_PlayerAchievement;
	PlayerSetup m_PlayerSetup;
	public static Prototype.NetworkLobby.PlayerDataPers _instance;
	public static Prototype.NetworkLobby.LobbyTopPanel _topPanel;

	void Start()
	{
		m_PlayerSetup = GetComponent<PlayerSetup> ();
		m_PlayerAchievement = GetComponent<PlayerAchievementCount> ();
	}

	public void SavePlayerData()
	{
		WWWForm form = new WWWForm ();

		form.AddField ("UserIDPost", m_PlayerSetup.m_PlayerName);
		//form.AddField ("UserPaswwordPost", m_PlayerSetup.m_PlayerPassword);
		form.AddField ("TotalXP", m_PlayerSetup.m_PlayerTotalXP);
		form.AddField ("SliderXP", m_PlayerSetup.m_XPSliderProgress);
		form.AddField ("IncomingXP", (int)m_PlayerAchievement.score);
		form.AddField ("TotalCoins", m_PlayerSetup.m_NumberofCoins);
		form.AddField ("TotalGems", m_PlayerSetup.m_NumberofGems);

		WWW www = new WWW ("http://databasehandler42.gearhostpreview.com/inc/saveplayerdata.php", form);

		SaveData data = new SaveData ();

		data.Username = m_PlayerSetup.m_PlayerName;
		data.Password = m_PlayerSetup.m_PlayerPassword;

		data.PlayerTotalXp = m_PlayerSetup.m_PlayerTotalXP;
		data.PlayerXPSliderValue = m_PlayerSetup.m_XPSliderProgress;
		data.PlayerincomingXPSliderValue = m_PlayerAchievement.score;

		data.NumberOfCoins = m_PlayerSetup.m_NumberofCoins + 0;
		data.NumberOfGems = m_PlayerSetup.m_NumberofGems + 0;

		string saveStatePath = Path.Combine(Application.persistentDataPath, "player"+data.Username+"Info.json");
		File.WriteAllText (saveStatePath, JsonUtility.ToJson (data, true));

		_instance.LoadUserDataLocal ();	//IENUMERATOR won't work if any script destroyed
		_topPanel.LoadPlayerData ();

	}
}


[Serializable]
class SaveData
{
	public string Username;
	public string Password;

	public int PlayerTotalXp;
	public float PlayerXPSliderValue;
	public float PlayerincomingXPSliderValue;

	public int NumberOfCoins;
	public int NumberOfGems;
}

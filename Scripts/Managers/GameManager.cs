
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour
{
	//------------------------------------GLOBAL VARIABLES-------------------------------------//
	static public string GameMode; 
	public Button ExitBtn;
	static public GameManager s_Instance;
	protected bool m_UIPanelControlCommand;		//Give Permission to control the UI, eg:-ScoreBoard Panel

	public CanvasGroup m_NotifierPanel;
	public RectTransform m_NotificationContentsPanel;

	int m_RoundNumber;

	float m_PurpleTeamScore;
	float m_YellowTeamScore;
	public RectTransform TopPanel;
	public Text m_PTS;
	public Text m_YTS;

	string message = "DRAW !";
	static Color WarningColor = new Color (227.0f / 255.0f, 91.0f / 255.0f, 88.0f / 255.0f, 1.0f);
	static Color RoundStartingColor = new Color(40.0f / 255.0f, 140 / 255.0f, 70.0f / 255.0f, 1.0f);
	static Color RoundWinColor = new Color(110.0f / 255.0f, 120 / 255.0f, 165.0f / 255.0f, 1.0f);
	static Color WiningColor = new Color(60.0f, 0.0f, 140.0f / 255.0f, 1.0f);
	//------------------------------------------------------------------------------------------//

	//-------------------------------------LOCAL_VARIABLES--------------------------------------//
	static public List<PlayerManager> m_Players = new List<PlayerManager> ();
	static public List<PlayerManager> m_PurpleTeam = new List<PlayerManager> ();
	static public List<PlayerManager> m_YellowTeam = new List<PlayerManager> ();


	static public List<PlayerAchievementCount> m_PlayerInGameAchievement = new List<PlayerAchievementCount> ();
	public EnemyManager m_Enemies;

	GameObject[] DesEnemy;
	int m_EnemyNumberperRound;
	int m_NumberofEnemies;
	public Transform[] m_SpawnPoint;
	public Transform[] m_OSpawnPoint;
	public bool isAllPlayerDied;

	float m_StartDelay = 5.0f;
	float m_EndDelay = 5.0f;
	public float MatchTime;
	private WaitForSeconds m_StartWait;
	private WaitForSeconds m_EndWait;

	public Text m_HeaderText;
	public Text m_RoundText;

	private PlayerManager m_RoundWinner;
	private PlayerManager m_GameWinner;

	public bool GameFinished = false;

	bool m_IsGameTransformOn = true;
	public RectTransform[] m_OnGameRecttransform;

	void Awake()
	{
		s_Instance = this;
		m_IsGameTransformOn = true;
	}

	[ServerCallback]
	private void Start()
	{
		m_StartWait = new WaitForSeconds (m_StartDelay);
		m_EndWait = new WaitForSeconds (m_EndDelay);

		if (GameMode == "ZMB-1" || GameMode == "ZMB-2")
			StartCoroutine (ZMBGameLoop ());
		else if (GameMode == "TDMM-1" || GameMode == "TDMM-2")
			StartCoroutine (TeamDeathMatchGameLoop());
	}

	//-----------------------------------ZOMBIES GAME LOOP---------------------------------//
	#region ZOMBIES_GAME_CONTROL

	private IEnumerator ZMBGameLoop()
	{
		while (m_Players.Count < 1) 
		{
			DisablePlayerControl ();
			RpcInGameNotifier (true, true, Color.red, "NEED ATLEAST 1 PLAYER TO START THE ROUND !");
			RpcMatchUIControl(false);	
			yield return null;
		}

		RpcMatchUIControl(false);

		for (int i = 0; i < 5; i++) 
		{
			RpcInGameNotifier (true, false, Color.blue,"SETTING UP PLAYERS");
			yield return new WaitForSeconds (10.0f);
			yield return StartCoroutine(ZMBRoundStarting ());
			yield return StartCoroutine(ZMBRoundPlaying ());
			yield return StartCoroutine(ZMBRoundEnding ());

			if (m_GameWinner != null) 
			{
				RpcInGameNotifier (true, false, WiningColor, "GAME WINNER IS " + m_GameWinner.m_PlayerName);
				yield return new WaitForSeconds (4.0f);
				RpcInGameNotifier (false, false, WiningColor, string.Empty);
				yield return new WaitForSeconds (6.0f);

				ClearPlayerList ();
				ClearScoreBoard ();
				OnClickExitButton();
			}
		}
	}

	private IEnumerator ZMBRoundStarting()
	{
		RpcZMBRoundStarting ();

		m_Enemies.ResetEnemies ();
		m_EnemyNumberperRound++;

		if (m_EnemyNumberperRound > 1)
			m_NumberofEnemies = (int)((m_EnemyNumberperRound * 5) + (m_Players.Count * 8));
		else
			m_NumberofEnemies = 5 * m_Players.Count;

		m_Enemies.SetEnemyNumber (m_NumberofEnemies);

		yield return m_StartWait;
	}

	[ClientRpc]
	void RpcZMBRoundStarting()
	{
		TopPanel.gameObject.SetActive (false);	//Disable the ScoreCount Panel

		ResetAllPlayers (false, 14);			//CanPlayerAlive and LayerMakNumber
		DisablePlayerControl ();
		SetupInventory ();

		m_RoundNumber++;
		InGameNotifier (true, false, RoundStartingColor, "ZOMBIES");

		if (m_RoundNumber == 5)
			m_RoundText.text = "FINAL ROUND";
		else
			m_RoundText.text = "ROUND " + m_RoundNumber;
	}

	private IEnumerator ZMBRoundPlaying()
	{
		RpcZMBRoundPlaying ();

		while (!WhoLeft ()) 
			yield return null;
	}

	[ClientRpc]
	void RpcZMBRoundPlaying()
	{
		EnablePlayerControl ();
		InGameNotifier (false, false, Color.clear, string.Empty);
		MatchUIControl(true);
	}

	private IEnumerator ZMBRoundEnding()
	{
		m_RoundWinner = null;
		m_RoundWinner = GetRoundWinner ();
		if (m_RoundWinner != null)
			m_RoundWinner.m_Wins++;
		
		m_GameWinner = GetGameWinner ();

		if (m_GameWinner != null)
			RpcFinishGame ();

		yield return new WaitForSeconds (1.0f);
		RpcInGameNotifier (true, false, RoundWinColor, "ROUND WINNER IS " + m_RoundWinner.m_PlayerName);
		RpcMatchUIControl(false);

		RpcZMBRoundEnding ();
		yield return m_EndWait;
	}

	[ClientRpc]
	void RpcFinishGame()
	{
		GameFinished = true;
	}

	[ClientRpc]
	void RpcZMBRoundEnding()
	{
		DisablePlayerControl ();
		SavePlayerProgress ();
		MatchUIControl(false);
	}

	#endregion
	//------------------------------------------------------------------------------------------//

	//----------------------------------------TEAM DEATH MATCH GAME LOOP-------------------------------------------------//

	#region TEAM_DEATH_MATCH

	private IEnumerator TeamDeathMatchGameLoop()
	{
		while (m_Players.Count < 2) 
		{
			DisablePlayerControl ();
			RpcInGameNotifier (true, true, Color.red, "NEED ATLEAST 2 PLAYER TO START THE ROUND !");
			RpcMatchUIControl(false);	
			yield return null;
		}

		RpcMatchUIControl(false);

		for (int i = 0; i < 1; i++) 
		{
			RpcInGameNotifier (true, false, Color.blue, "SETTING UP PLAYERS");
			yield return new WaitForSeconds (10.0f);
			yield return StartCoroutine (TDMSetupPlayers());
			yield return new WaitForSeconds (2.0f);
			yield return StartCoroutine(TDMRoundStarting ());
			yield return StartCoroutine(TDMRoundPlaying ());
			yield return StartCoroutine(TDMRoundEnding ());

			if (m_GameWinner != null) 
			{
				InGameNotifier (true, false, WiningColor, "GAME WINNER IS " + m_GameWinner.m_PlayerName);
				yield return new WaitForSeconds (4.0f);
				InGameNotifier (false, false, WiningColor, string.Empty);
				yield return new WaitForSeconds (6.0f);
				OnClickExitButton();
			}
		}
	}

	private IEnumerator TDMSetupPlayers()
	{
		RpcTDMSetupPlayers ();
		yield return null;
	}

	/// <summary>
	/// Setting up the players (eg: Their color, inventory)
	/// </summary>
	[ClientRpc]
	private void RpcTDMSetupPlayers()
	{
		//SetupInventory ();
		int EachTeamPlayer = 0, n = 0, UniquePlayer = 0, PurpleTeam = 0, YellowTeam = 0;

		EachTeamPlayer = (int)m_Players.Count / 2;
		n = Random.Range (0, 2);

		if (n == 0) 
		{
			PurpleTeam = EachTeamPlayer;
			YellowTeam = m_Players.Count - EachTeamPlayer;
		}
		else if (n == 1) 
		{
			YellowTeam = EachTeamPlayer;
			PurpleTeam = m_Players.Count - EachTeamPlayer;
		}

		for (int i = 0; i < m_Players.Count; i++) 
		{

			DoFirstStep:

			UniquePlayer = Random.Range (0, 2);

			if (m_PurpleTeam.Count == PurpleTeam && PurpleTeam > 0) 
			{
				m_YellowTeam.Add (m_Players [i]);
				goto EndIt;

			}
			else if (m_YellowTeam.Count == YellowTeam && YellowTeam > 0) 
			{
				m_PurpleTeam.Add (m_Players[i]);
				goto EndIt;
			}

			if (UniquePlayer == 0 && PurpleTeam != 0)
				m_PurpleTeam.Add (m_Players [i]);
			else if (UniquePlayer == 1 && YellowTeam != 0)
				m_YellowTeam.Add (m_Players [i]);
			else
				goto DoFirstStep;

			EndIt:;
		}

		if (m_PurpleTeam.Count > 0) 
		{
			for (int j = 0; j < m_PurpleTeam.Count; j++)
				m_PurpleTeam [j].SetColor (1);
		}
		if (m_YellowTeam.Count > 0) 
		{
			for (int k = 0; k < m_YellowTeam.Count; k++)
				m_YellowTeam [k].SetColor (2);
		}

	}

	private IEnumerator TDMRoundStarting()
	{
		//m_Enemies.enabled = false;
		RpcTDMRoundStarting ();
		yield return m_StartWait;
	}

	[ClientRpc]
	void RpcTDMRoundStarting()
	{
		ResetTeamPlayers (true);			//CanPlayerAlive
		TopPanel.gameObject.SetActive(true);
		DisablePlayerControl ();
		SetupInventory ();

		m_RoundNumber++;

		InGameNotifier (true, false, RoundStartingColor, "TEAM DEATHMATCH");
		m_RoundText.text = "ROUND " + m_RoundNumber;
	}

	private IEnumerator TDMRoundPlaying()
	{
		RpcTDMRoundPlaying ();
		yield return new WaitForSeconds (600.0f);
	}

	[ClientRpc]
	void RpcTDMRoundPlaying()
	{
		EnablePlayerControl ();
		InGameNotifier (false, false, Color.clear, string.Empty);
		MatchUIControl(true);
	}

	private IEnumerator TDMRoundEnding()
	{
		m_RoundWinner = null;
		m_RoundWinner = TDMGetRoundWinner ();

		if (m_RoundWinner != null) 
		{
			if (m_RoundWinner.m_Setup.MaterialColorIndex == 1)
				for(int i = 0; i < m_PurpleTeam.Count; i++)
					m_RoundWinner.m_PurpleWins++;
			if (m_RoundWinner.m_Setup.MaterialColorIndex == 2)
				for(int i = 0; i < m_YellowTeam.Count; i++)
					m_RoundWinner.m_YellowWins++;
		}

		m_GameWinner = TDMGetGameWinner ();

		if (m_GameWinner != null)
			RpcFinishGame ();

		yield return new WaitForSeconds (1.0f);

		RpcInGameNotifier (true, false, RoundWinColor, message+" WON THE ROUND");
		RpcMatchUIControl(false);

		RpcTDMRoundEnding ();

		yield return m_EndWait;
	}

	[ClientRpc]
	void RpcTDMRoundEnding()
	{
		DisablePlayerControl ();
		SavePlayerProgress ();
		MatchUIControl(false);
	}

	#endregion
	//-----------------------------------------------------------------------------------------//


	void Update()
	{
		//ShowScore ();

		if (Input.GetKeyDown (KeyCode.Escape))
			ToggleTransform ();
	}

	void ToggleTransform()
	{
		if (!m_UIPanelControlCommand)
			return;
		
		if (m_IsGameTransformOn) 
		{
			UIPanelManager.s_Singleton.isAfterScenePanelOn = m_IsGameTransformOn;
			UIPanelManager.s_Singleton.DisablePanel ();

			foreach (RectTransform rt in m_OnGameRecttransform)
				rt.gameObject.SetActive (!m_IsGameTransformOn);
			m_IsGameTransformOn = !m_IsGameTransformOn;
			return;
		}

		if (!m_IsGameTransformOn) 
		{
			UIPanelManager.s_Singleton.isAfterScenePanelOn = m_IsGameTransformOn;
			UIPanelManager.s_Singleton.DisablePanel ();

			foreach (RectTransform rt in m_OnGameRecttransform)
				rt.gameObject.SetActive (!m_IsGameTransformOn);
			m_IsGameTransformOn = !m_IsGameTransformOn;
			return;
		}
	}

	static public void AddPlayer(GameObject player, int playerNumber, string name, string Userpass, int TotalXP ,int SliderValue, int NoCoins, int NoGems, int LoadoutNumber)
	{
		PlayerManager tmp = new PlayerManager ();
		tmp.m_Instance = player;
		tmp.m_PlayerNumber = playerNumber;
		tmp.m_PlayerName = name;
		tmp.m_PlayerPass = Userpass;
		tmp.m_TotalXP = TotalXP;
		tmp.m_XPSliderValue = SliderValue;
		tmp.m_NumberofCoin = NoCoins;
		tmp.m_NumberofGems = NoGems;
		tmp.m_LoadoutNumber = LoadoutNumber;

		tmp.Setup ();

		m_Players.Add (tmp);
		m_PlayerInGameAchievement.Add (player.GetComponent<PlayerAchievementCount> ());
	}
	public void RemovePlayer(GameObject player)
	{
		PlayerManager toRemove = null;
		foreach (var tmp in m_Players) 
		{
			if (tmp.m_Instance == player) 
			{
				toRemove = tmp;
				break;
			}
		}
		if (toRemove != null)
			m_Players.Remove (toRemove);
			
	}
	public static List<PlayerAchievementCount> GetAllPlayers()
	{
		//return m_PlayerInGameAchievement.ToArray ();
		return m_PlayerInGameAchievement;
	}
		
	private bool OnePlayerLeft()
	{
		int numPlayerLeft = 0;

		for (int i = 0; i < m_Players.Count; i++) 
		{
			if (m_Players [i].m_PlayerComponents.activeSelf)
				numPlayerLeft++;
		}
		return numPlayerLeft <= 1;
	}

	/// <summary>
	/// Keep loop the function, 
	/// </summary>
	/// <returns><c>true</c>, will return if ALL ENEMY WILL DIE, MATCH TIME OVER, ALL PLAYER DIE, <c>false</c> otherwise, it will continue</returns>
	private bool WhoLeft()
	{
		int isTrue = 0;

		if(m_Enemies.m_currentEnemyCount <= 0)
		{
			isTrue = 0; 
			return isTrue <= 0;
		}
		if (MatchTime >= 600)
			return true;
		for (int i = 0; i < m_Players.Count; i++) 
		{
			if (m_Players [i].m_Instance != null)				//Check if a player still in Game otherwise it will ignore the player's instance
			{				
				if (m_Players [i].m_PlayerComponents.activeSelf)
					isTrue++;	
				if (isTrue < 1)
					isAllPlayerDied = true;
			}
				
		}
		return isTrue <= 0;
	}

	/// <summary>
	/// Resets all players.
	/// </summary>
	/// <param name="CanPlayerAlive">If set to <c>true</c> Players will spawn again.</param>
	/// <param name="layermask">Layermask of Player which will decide wheather will killed by another player or not.</param>
	private void ResetAllPlayers(bool CanPlayerAlive, int layermask)
	{
		for (int i = 0; i < m_Players.Count; i++)
		{
			m_Players[i].m_SpawnPoint = m_SpawnPoint[m_Players[i].m_Setup.m_PlayerNumber];
			m_Players[i].Reset(CanPlayerAlive,layermask);
		}
	}

	private void ResetTeamPlayers(bool CanAlive)
	{
		if (m_PurpleTeam.Count > 0) 
		{
			for (int i = 0; i < m_PurpleTeam.Count; i++) 
			{
				m_PurpleTeam [i].m_SpawnPoint = m_SpawnPoint [m_PurpleTeam [i].m_Setup.m_PlayerNumber];
				m_PurpleTeam [i].Reset (CanAlive, 15);
			}
		}

		if (m_YellowTeam.Count > 0) 
		{
			for (int i = 0; i < m_YellowTeam.Count; i++) 
			{
				m_YellowTeam [i].m_SpawnPoint = m_OSpawnPoint [m_YellowTeam [i].m_Setup.m_PlayerNumber];
				m_YellowTeam [i].Reset (CanAlive, 16);
			}
		}
	}
	private void SetupInventory()
	{
		for (int i = 0; i < m_Players.Count; i++)
		{
			m_Players[i].inventory();
		}
	}
	private void SavePlayerProgress()
	{
		for (int i = 0; i < m_Players.Count; i++)
		{
			m_Players[i].SaveProgress();
		}
	}
	private PlayerManager GetRoundWinner()
	{
		PlayerManager _roundnWinner = m_Players[0] ;
		for (int i = 0; i < m_Players.Count; i++)
		{
			if (m_Players [i].m_Instance != null) 
			{
				if (m_Players [i].m_PlayerComponents.activeSelf) 
				{
					if (_roundnWinner.m_PlayerAchievement.kill < m_Players [i].m_PlayerAchievement.kill)
						_roundnWinner = m_Players [i];
				}
			}
		}
		return _roundnWinner;
	}

	/// <summary>
	/// Gets the game winner.
	/// </summary>
	/// <returns>The game winner with Team Color.</returns>
	private PlayerManager TDMGetGameWinner()
	{
		int TeamMaxScore = 0;
		PlayerManager gameWinner = m_Players[0];

		if (m_RoundNumber == 5) 
		{
			for (int i = 0; i < m_Players.Count; i++) 
			{
				if (m_Players [i].m_Instance != null) 
				{
					if (m_Players [i].m_PurpleWins > TeamMaxScore)
						TeamMaxScore = m_Players [i].m_PurpleWins;
					if (m_Players [i].m_YellowWins > TeamMaxScore)
						TeamMaxScore = m_Players [i].m_YellowWins;
				}
			}

			for(int j = 0; j < m_Players.Count; j++)
			{
				if (m_Players [j].m_Instance != null) 
				{
					if (m_Players [j].m_PurpleWins == TeamMaxScore)
						gameWinner = m_Players [j];
					if (m_Players [j].m_YellowWins == TeamMaxScore)
						gameWinner = m_Players [j];
				}
			}

			return gameWinner;
		}

		return null;
	}
	private PlayerManager TDMGetRoundWinner()
	{
		float TotalKillByPurpleTeam = 0.0f, TotalKillByYellowTeam = 0.0f;
		PlayerManager roundWinner = m_Players [0];

		if (m_PurpleTeam.Count > 0) 
		{
			for (int i = 0; i < m_PurpleTeam.Count; i++)
				TotalKillByPurpleTeam += m_PurpleTeam [i].m_PlayerAchievement.kill;
		}
		if (m_YellowTeam.Count > 0) 
		{
			for (int j = 0; j < m_YellowTeam.Count; j++)
				TotalKillByYellowTeam += m_YellowTeam [j].m_PlayerAchievement.kill;
		}

		if (TotalKillByPurpleTeam > TotalKillByYellowTeam) {
			message = "PURPLE TEAM";
			for (int i = 0; i < m_PurpleTeam.Count; i++) {
				if (roundWinner.m_PlayerAchievement.kill < m_PurpleTeam [i].m_PlayerAchievement.kill)
					roundWinner = m_PurpleTeam [i];
			}
		} else if (TotalKillByYellowTeam > TotalKillByPurpleTeam) {
			message = "YELLOW TEAM";
			for (int i = 0; i < m_YellowTeam.Count; i++) {
				if (roundWinner.m_PlayerAchievement.kill < m_YellowTeam [i].m_PlayerAchievement.kill)
					roundWinner = m_YellowTeam [i];
			}
		} else
			message = "DRAW !";

		return roundWinner;

	}

	private PlayerManager GetGameWinner()
	{
		int maxScore = 0;
		if (m_RoundNumber == 5 || isAllPlayerDied) 
		{
			for (int i = 0; i < m_Players.Count; i++) 
			{
				if (m_Players [i].m_Instance != null) 
				{
					if (m_Players [i].m_Wins > maxScore)
						maxScore = m_Players [i].m_Wins;
				}
			}

			for (int i = 0; i < m_Players.Count; i++) 
			{
				if (m_Players [i].m_Instance != null) 
				{
					if (m_Players [i].m_Wins == maxScore)
						return m_Players [i];
				}
			}
		}
		return null;
	}
	private void EnablePlayerControl()
	{
		for (int i = 0; i < m_Players.Count; i++)
		{
			m_Players[i].EnableControl();
		}
	}
	private void DisablePlayerControl()
	{
		for (int i = 0; i < m_Players.Count; i++)
		{
			m_Players[i].DisableControl();
		}
	}

	/// <summary>
	/// Notifier will display in all clients
	/// </summary>
	/// <param name="NotifierActive">If set to <c>true</c> notifier active.</param>
	/// <param name="ExitButtonActive">If set to <c>true</c> exit button active.</param>
	/// <param name="color">Color.</param>
	/// <param name="HeaderMessage">This message will show in big letter.</param>
	[ClientRpc]
	private void RpcInGameNotifier(bool NotifierActive, bool ExitButtonActive, Color color, string HeaderMessage)
	{
		InGameNotifier (NotifierActive, ExitButtonActive, color, HeaderMessage);
	}

	private void InGameNotifier(bool NotifierActive, bool ExitButtonActive, Color color, string HeaderMessage)
	{
		m_NotifierPanel.gameObject.SetActive (NotifierActive);

		m_NotificationContentsPanel.gameObject.SetActive (NotifierActive);
		m_NotificationContentsPanel.GetComponent<Image> ().color = color;
		m_HeaderText.text = HeaderMessage;
		ExitBtn.gameObject.SetActive (ExitButtonActive);

		ExitBtn.onClick.RemoveAllListeners ();
		ExitBtn.onClick.AddListener (RpcOnClickExitButton);
	}


	[ClientRpc]
	private void RpcOnClickExitButton()
	{
		OnClickExitButton ();
	}

	void OnClickExitButton()
	{
		LobbyManager.s_Singleton.BackToMainMenuScene (true);
		LobbyManager.s_Singleton.ServerReturnToLobby ();
	}
	public void GetReadyToQuite()
	{
		m_Players.Clear ();
		SGetReadyToQuite ();
	}
	[Server]
	void SGetReadyToQuite()
	{
		DesEnemy = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject ob in DesEnemy)
			Destroy (ob);
		ClearScoreBoard ();
		ClearPlayerList ();
		if(GameMode == "ZMB-1" || GameMode == "ZMB-2")
			m_Enemies.gameObject.SetActive (false);
		RpcKickAllClient ();
		this.gameObject.SetActive (false);

	}
	public void ClearPlayerList()
	{
		m_Players.Clear ();
	}
	void RpcKickAllClient()
	{
		LobbyManager.s_Singleton.LeaveLobbyButton();
	}

	public void ClearScoreBoard()
	{
		ScoreBoard.Instance.ClearList ();
	}

	/// <summary>
	/// It can give permission or permite a user to use the OnMatch UI.
	/// </summary>
	[ClientRpc]
	private void RpcMatchUIControl(bool granted)
	{
		MatchUIControl (granted);
	}

	/// <summary>
	/// It can give permission or permite a user to use the OnMatch UI.
	/// </summary>
	public void MatchUIControl(bool granted)
	{
		m_UIPanelControlCommand = granted;
	}


	#region Listeners

	/// <summary>
	/// Increase the purpleTeam winn by 1.
	/// </summary>
	protected void PurpleTeamWinIncrement()
	{
		for (int i = 0; i < m_PurpleTeam.Count; i++)
		{
			if (m_PurpleTeam [i].m_Instance != null)
				m_PurpleTeam [i].m_PurpleWins++;
		}
	}

	/// <summary>
	/// Increase the yellow team winn by 1.
	/// </summary>
	protected void YellowTeamWinIncrement()
	{
		for (int i = 0; i < m_YellowTeam.Count; i++)
		{
			if (m_YellowTeam [i].m_Instance != null)
				m_YellowTeam [i].m_YellowWins++;
		}
	}

	/// <summary>
	/// Called on server
	/// </summary>
	[Server]
	protected void DisplayScoreOnServer()
	{
		RpcShowScore ();
	}

	#endregion


	#region Methods

	/// <summary>
	/// Display the score.
	/// Called from PlayerHealth Script when a player die or when Game is over.
	/// </summary>
	public void DisplayScore()
	{
		DisplayScoreOnServer();
	}

	/// <summary>
	/// u
	/// </summary>
	[ClientRpc]
	private void RpcShowScore()
	{
		m_PurpleTeamScore = 0;
		m_YellowTeamScore = 0;

		for(int i = 0; i < m_PurpleTeam.Count; i++)
			m_PurpleTeamScore += m_PurpleTeam [i].m_PlayerAchievement.kill;
		for(int j = 0; j < m_YellowTeam.Count; j++)
			m_YellowTeamScore += m_YellowTeam [j].m_PlayerAchievement.kill;

		m_PTS.text = ""+m_PurpleTeamScore;
		m_YTS.text = ""+m_YellowTeamScore;
	}

	#endregion
}
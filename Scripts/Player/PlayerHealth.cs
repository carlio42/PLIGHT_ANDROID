using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;


//THIS SCRIPT USED IN SURVIVAL - ONLINE MULTIPLAYER
public class PlayerHealth : NetworkBehaviour
{
	public Slider m_PlayerHealthSlider;				//Slider which will show Only on localPlayer Screen
	public Slider m_InPlayerHealthSlider;			//Other players slider which will show Only to localPlayer
	public Text m_InPlayerNameText;					//Other players name which will show Only to localPlayer
	public RectTransform WeaponUI;
	private PlayerUserControl m_PlayerUserControl;
	private PlayerShooting m_PlayerShooting;

	public const float m_StartingHealth = 100.0f;		//Player Starting Health

	[SyncVar(hook = "OnChangehealth")]				//Synchronize in Network
	public float m_CurrentHealth = m_StartingHealth;	//Store the Maximum health of player in current health at begning

	public NetworkStartPosition[] spawnPoints;		//Network Player spawn positions

	public bool playerCanAlive = false;

	[HideInInspector]
	public bool m_IsPlayerDead = false;				//Is the player dead ?
	public CapsuleCollider m_Collider;					//Used so Player doesn't collider with anything after it's dead.
	public GameObject[] m_PlayerUI;					//UI of Player, used to disabl after dead
	public GameObject m_PlayerBody;					//Components of Player, used to disabl after dead
	public RectTransform UIPanel;

	void Awake()
	{
		m_Collider = GetComponent<CapsuleCollider> ();
		m_PlayerUserControl = GetComponent<PlayerUserControl>();
		m_PlayerShooting = GetComponent<PlayerShooting>();
	}

	void Start()
	{
		if (!isLocalPlayer) 
		{
			m_PlayerHealthSlider.gameObject.SetActive (false);	//Disable the Other Player's Slider which is show to local Player
			m_InPlayerHealthSlider.gameObject.SetActive (true);	//Enable the other Player's Slider which is show by all Player
			m_InPlayerNameText.gameObject.SetActive(true);
			WeaponUI.gameObject.SetActive (false);
		}

		else 
		{
			m_PlayerHealthSlider.gameObject.SetActive (true);		//Disable the Other Player's Slider which is show to local Player
			m_InPlayerHealthSlider.gameObject.SetActive (false);	//Disable the other Player's Slider which is show by all Player
			m_InPlayerNameText.gameObject.SetActive(false);
			WeaponUI.gameObject.SetActive (true);
		}
	}

	void Update()
	{
		if (!isLocalPlayer)
			UIPanel.rotation = Camera.main.transform.rotation;
	}

	public void TakeDamageToPlayer(float amount,GameObject EnemyPlayer)
	{
		if (!isServer)
			return;

		//Reduce current health by the amount of damage done.
		m_CurrentHealth -= amount;

		if (EnemyPlayer.GetComponent<PlayerAchievementCount> ()) 
			EnemyPlayer.GetComponent<PlayerAchievementCount> ().LocalPlayerScoreCount (0.5f);
		//if health is zero or below zero call PlayerDeath funtion.
		if (m_CurrentHealth <= 0) 
		{
			if (EnemyPlayer.GetComponent<PlayerAchievementCount> ()) 
			{
				EnemyPlayer.GetComponent<PlayerAchievementCount> ().LocalPlayerScoreCount (30);
				EnemyPlayer.GetComponent<PlayerAchievementCount> ().LocalPlayerKiilCount (1);
				GetComponent<PlayerAchievementCount> ().LocalPlayerDeathCount (1);
			}
			GameManager.s_Instance.DisplayScore ();
			StartCoroutine (PlayerDeath ());
		}
	}

	void OnChangehealth(float newHealth)	
	{		
		m_CurrentHealth = newHealth;
		m_PlayerHealthSlider.value = newHealth;
		m_InPlayerHealthSlider.value = newHealth;
	}

	IEnumerator PlayerDeath()
	{
		m_IsPlayerDead = true;
		RpcEnableDeathUIPanels();
		//GameManager.s_Instance.KillCount ();

		RpcSetPlayerActive (false);			//Set the Player deactive after Dead

		yield return new WaitForSeconds (8);

		if (playerCanAlive) 
		{
			m_CurrentHealth = m_StartingHealth;
			RpcRespawn ();
			RpcSetPlayerActive (true);
			m_IsPlayerDead = false;
		}
	}

	[ClientRpc]
	void RpcSetPlayerActive(bool active)
	{
		DisablePlayerComponents (active);
	}

	private void DisablePlayerComponents(bool active)
	{
		m_Collider.enabled = active;

		foreach (GameObject Go in m_PlayerUI) 
			Go.SetActive (active);

		m_PlayerBody.gameObject.SetActive (active);

		if (active) 
		{
			Start ();
			this.enabled = true;
			m_PlayerUserControl.enabled = active;
			m_PlayerShooting.enabled = active;
		} 
		else
		{
			m_PlayerUserControl.enabled = active;
			m_PlayerShooting.enabled = active;
		}
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if (!isLocalPlayer)
			return;

		if (spawnPoints != null && spawnPoints.Length > 0)
			transform.position = spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position;
	}

	[ClientRpc]
	void RpcEnableDeathUIPanels()
	{
		if (!isLocalPlayer)
			return;
		if (GameManager.s_Instance.GameFinished)
			StartCoroutine (ControlUIPanelManager (12.0f, 4.0f));
		else
			StartCoroutine (ControlUIPanelManager(1.0f, 6.0f));
	}

	IEnumerator ControlUIPanelManager(float StartTime, float EndTime)
	{
		yield return StartCoroutine (EnableScoreBoard (StartTime, EndTime));
		yield return StartCoroutine (DisableScoreBoard ());
	}

	IEnumerator EnableScoreBoard(float StartTime, float EndTime)
	{
		yield return new WaitForSeconds (StartTime);

		GameManager.s_Instance.MatchUIControl(false);
		UIPanelManager.s_Singleton.ScoreBoardPanel (true);

		yield return new WaitForSeconds (EndTime);
	}
	IEnumerator DisableScoreBoard()
	{
		GameManager.s_Instance.MatchUIControl(true);
		UIPanelManager.s_Singleton.ScoreBoardPanel (false);
		yield return null;
	}

	public void SetDefaults()
	{
		m_CurrentHealth = m_StartingHealth;
		m_IsPlayerDead = false;
		DisablePlayerComponents (true);
	}
}
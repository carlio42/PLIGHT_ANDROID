using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField]
public class PlayerManager
{
	//This class is to manage various setting on Player.
	//It works with GameManager and weather or not player have control of their character in the different phase or not
	public Transform m_SpawnPoint;
	[HideInInspector]
	public GameObject m_Instance;
	[HideInInspector]
	public GameObject m_PlayerComponents;

	public string m_PlayerName;
	public int m_PlayerNumber;
	public int m_LocalPlayerID;

	public string m_PlayerPass;
	public int m_TotalXP;
	public int m_XPSliderValue;
	public int m_NumberofCoin;
	public int m_NumberofGems;
	public int m_LoadoutNumber;

	public PlayerUserControl m_MovementControl;
	public PlayerShooting m_Shooting;
	public PlayerHealth m_Health;
	public PlayerSetup m_Setup;
	public SavePlayerProgressOnline m_SavePlayerProgressOnline;
	public PlayerAchievementCount m_PlayerAchievement;
	public WeaponManager m_PlayerInventory;

	[HideInInspector]
	public int m_Wins;
	[HideInInspector]
	public int m_PurpleWins;
	[HideInInspector]
	public int m_YellowWins;
	//[HideInInspector]
	//public int m_TotalWinsTDM;

	public void Setup()
	{
		m_MovementControl = m_Instance.GetComponent<PlayerUserControl> ();
		m_Shooting = m_Instance.GetComponent<PlayerShooting> ();
		m_Health = m_Instance.GetComponent<PlayerHealth> ();
		m_Setup = m_Instance.GetComponent<PlayerSetup> ();
		m_SavePlayerProgressOnline = m_Instance.GetComponent<SavePlayerProgressOnline> ();
		m_PlayerAchievement = m_Instance.GetComponent<PlayerAchievementCount> ();
		m_PlayerInventory = m_Instance.GetComponent<WeaponManager> ();

		m_PlayerComponents = m_Health.m_PlayerBody;
		//m_Health.m_Manager = this;

		//Setup is use for diverse Network Related sync
		m_Setup.m_PlayerName = m_PlayerName;
		m_Setup.m_PlayerNumber = m_PlayerNumber;

		m_Setup.m_PlayerPassword = m_PlayerPass;
		m_Setup.m_PlayerTotalXP = m_TotalXP;
		m_Setup.m_XPSliderProgress = m_XPSliderValue;
		m_Setup.m_NumberofCoins = m_NumberofCoin;
		m_Setup.m_NumberofGems = m_NumberofGems;
		m_Setup.m_LoadoutNumber = m_LoadoutNumber;
	}

	public void DisableControl()
	{
		m_MovementControl.enabled = false;
		//m_Shooting.enabled = false;
	}

	public void EnableControl()
	{
		m_MovementControl.enabled = true;

		//m_Shooting.enabled = true;
	}

	//Used at the start of each rounf to put the player into default state
	public void Reset(bool CanPlayerAlive, int layermask)
	{
		m_MovementControl.SetDeafults ();
		m_Shooting.SetDefaults (layermask);
		m_Health.SetDefaults ();
		m_Health.playerCanAlive = CanPlayerAlive;

		if (m_MovementControl.hasAuthority) 
		{
			m_MovementControl.m_Rigidbody.position = m_SpawnPoint.position;
			m_MovementControl.m_Rigidbody.rotation = m_SpawnPoint.rotation;
		}
	}

	public void SaveProgress()
	{
		m_SavePlayerProgressOnline.SavePlayerData ();
		m_PlayerInventory.SavePlayerInventoryData ();
	}
	public void inventory()
	{
		m_PlayerInventory.RunSetupInventory ();
		m_Shooting.SetupShooting ();
	}
	public void SetColor(int playerColor)
	{
		m_Setup.MeshColor (playerColor);
	}

}

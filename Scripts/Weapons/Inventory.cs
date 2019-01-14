using System.Collections;
using UnityEngine;
using System;
using System.IO;

public class Inventory : MonoBehaviour 
{
	public WeaponObject[] weapons;
	public int[] m_Loadout1,m_Loadout2,m_Loadout3,m_Loadout4,m_Loadout5,m_Loadout6;

	public int m_SelectedLoadout;
	public static Prototype.NetworkLobby.PlayerDataPers _instance;
	public static WeaponObject m_WeaponObject;

	void Start()
	{
		Prototype.NetworkLobby.LobbyPlayer.m_inventory = this; 
		WeaponManager._instance = this;
		LoadInventory ();
	}

	private void LoadInventory()
	{
		
		if (_instance.UsernameInput.text == null || _instance.UsernameInput.text == string.Empty)
			return;
		string saveStatePath = Path.Combine(Application.persistentDataPath, "inventory"+_instance.UsernameInput.text+"Info.json");

		if (File.Exists (saveStatePath)) 
		{
			_PlayerInventoryData iData = new _PlayerInventoryData ();

			string dataJSON = File.ReadAllText (saveStatePath);
			iData = JsonUtility.FromJson<_PlayerInventoryData>(dataJSON);

			weapons[1].isUnlocked = iData.Pistol_MK1;
			weapons[1].MaxBulletes = iData.Pistol_MK1Ammo;

			weapons[2].isUnlocked = iData.AssaultRifle_MK1;
			weapons[2].MaxBulletes = iData.AssaultRifle_MK1Ammo;

			weapons[3].isUnlocked = iData.ShotGun_MK1;
			weapons[3].MaxBulletes = iData.ShotGun_MK1Ammo;

			weapons[4].isUnlocked = iData.SMG;
			weapons[4].MaxBulletes = iData.SMG_Ammo;

			weapons[5].isUnlocked = iData.MachineGun;
			weapons[5].MaxBulletes = iData.MachineGun_Ammo;

			weapons[6].isUnlocked = iData.Sniper_MK1;
			weapons[6].MaxBulletes = iData.Sniper_MK1Ammo;

			m_Loadout1 = iData.Loadout1;
			m_Loadout2 = iData.Loadout2;
			m_Loadout3 = iData.Loadout3;
			m_Loadout4 = iData.Loadout4;
			m_Loadout5 = iData.Loadout5;
			m_Loadout6 = iData.Loadout6;

			m_SelectedLoadout = iData.MakeDefaultLoadout;
			RecalculateWeaponMagazine ();

		}
	}

	private void RecalculateWeaponMagazine ()
	{
		if (!weapons[1].isUnlocked)
			return;
		
		if (weapons [1].Magazine < 15) 
		{
			if (weapons [1].MaxBulletes < weapons [1].Magazine || weapons [1].MaxBulletes < 15)
				return;
			else 
			{
				int bullets = 0;

				bullets = 15 - weapons [1].Magazine;
				weapons [1].MaxBulletes -= bullets;
				weapons [1].Magazine += bullets;
			}

		}
		else if (weapons [2].Magazine < 40) 
		{
			if (!weapons[2].isUnlocked)
				return;
			
			if (weapons [2].MaxBulletes < weapons [2].Magazine || weapons [2].MaxBulletes < 40)
				return;
			else 
			{
				int bullets = 0;

				bullets = 40 - weapons [2].Magazine;
				weapons [2].MaxBulletes -= bullets;
				weapons [2].Magazine += bullets;
			}
		}
		else if (weapons [3].Magazine < 10) 
		{
			if (!weapons[3].isUnlocked)
				return;

			if (weapons [3].MaxBulletes < weapons [3].Magazine || weapons [3].MaxBulletes < 10)
				return;
			else 
			{
				int bullets = 0;

				bullets = 10 - weapons [3].Magazine;
				weapons [3].MaxBulletes -= bullets;
				weapons [3].Magazine += bullets;
			}
		}
		else if (weapons [4].Magazine < 25) 
		{
			if (!weapons[4].isUnlocked)
				return;

			if (weapons [4].MaxBulletes < weapons [4].Magazine || weapons [4].MaxBulletes < 25)
				return;
			else 
			{
				int bullets = 0;

				bullets = 25 - weapons [4].Magazine;
				weapons [4].MaxBulletes -= bullets;
				weapons [4].Magazine += bullets;
			}
		}
		else if (weapons [5].Magazine < 200) 
		{
			if (!weapons[5].isUnlocked)
				return;

			if (weapons [5].MaxBulletes < weapons [4].Magazine || weapons [5].MaxBulletes < 200)
				return;
			else 
			{
				int bullets = 0;

				bullets = 200 - weapons [5].Magazine;
				weapons [5].MaxBulletes -= bullets;
				weapons [2].Magazine += bullets;
			}
		}
		else if (weapons [6].Magazine < 12) 
		{
			if (!weapons[6].isUnlocked)
				return;

			if (weapons [6].MaxBulletes < weapons [6].Magazine || weapons [6].MaxBulletes < 12)
				return;
			else 
			{
				int bullets = 0;

				bullets = 12 - weapons [6].Magazine;
				weapons [6].MaxBulletes -= bullets;
				weapons [6].Magazine += bullets;
			}
		}
	}
}


[Serializable]
class _PlayerInventoryData
{
	public bool Pistol_MK1 = false;
	public int Pistol_MK1Ammo = 50;

	public bool AssaultRifle_MK1 = false;
	public int AssaultRifle_MK1Ammo = 0;

	public bool ShotGun_MK1 = false;
	public int ShotGun_MK1Ammo = 0;

	public bool SMG = false;
	public int SMG_Ammo = 0;

	public bool MachineGun = false;
	public int MachineGun_Ammo = 0;

	public bool Sniper_MK1 = false;
	public int Sniper_MK1Ammo = 0;

	public int[] Loadout1;
	public int[] Loadout2;
	public int[] Loadout3;
	public int[] Loadout4;
	public int[] Loadout5;
	public int[] Loadout6;

	public int MakeDefaultLoadout = 1;
}

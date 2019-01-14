using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class WeaponManager : NetworkBehaviour
{
	public WeaponObject[] weapons;
	//public GameObject[] m_WeaponGraphics;
	PlayerShooting m_Playershooting;
	PlayerSetup m_PlayerSetup;
	public static Inventory _instance;
	bool isReloading = false;

	public override void OnStartClient()
	{
		m_PlayerSetup = GetComponent<PlayerSetup> ();
		m_Playershooting = GetComponent<PlayerShooting> ();
		//StartCoroutine (SetWeapons());
	}
	public void RunSetupInventory()
	{
		StartCoroutine (SetWeapons());
	}
	public void Reload(WeaponObject m_CurrentWeapon)
	{		
		if (isReloading)
			return;
		StartCoroutine (StartReloading(m_CurrentWeapon));
	}

	IEnumerator StartReloading(WeaponObject m_CurrentWeapon)
	{
		int Magazine = 0;
	
		isReloading = true;
		yield return new WaitForSeconds (m_CurrentWeapon.ReloadTime);

		if (m_CurrentWeapon.MaxBulletes < m_CurrentWeapon.Magazine)
			yield return null;
		
		Magazine = WeaponMagazine (m_CurrentWeapon);
		m_CurrentWeapon.MaxBulletes -= Magazine;
		m_CurrentWeapon.Magazine = Magazine;

		weapons[m_CurrentWeapon.WeaponIndex].MaxBulletes = m_CurrentWeapon.MaxBulletes;

		isReloading = false;
	}



	public IEnumerator SetWeapons()
	{
		yield return new WaitForSeconds (1.0f);

		string saveStatePath = Path.Combine (Application.persistentDataPath, "inventory" + m_PlayerSetup.m_PlayerName + "Info.json");
		m_Playershooting.m_PrimaryWeapon = weapons [1];
		m_Playershooting.m_SecondaryWeapon = weapons [2];

		if (File.Exists (saveStatePath)) 
		{
			_PlayerInventoryData iData = new _PlayerInventoryData ();

			string dataJSON = File.ReadAllText (saveStatePath);

			iData = JsonUtility.FromJson<_PlayerInventoryData> (dataJSON);
			if (m_PlayerSetup.m_LoadoutNumber == 1) 
			{			
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout1 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout1 [1]];
			}
			else if (m_PlayerSetup.m_LoadoutNumber == 2) 
			{
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout2 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout2 [1]];
			}
			else if (m_PlayerSetup.m_LoadoutNumber == 3) 
			{
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout3 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout3 [1]];
			}
			else if (m_PlayerSetup.m_LoadoutNumber == 4) 
			{
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout4 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout4 [1]];
			}
			else if (m_PlayerSetup.m_LoadoutNumber == 5) 
			{
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout5 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout5 [1]];
			}
			else if (m_PlayerSetup.m_LoadoutNumber == 6) 
			{
				m_Playershooting.m_PrimaryWeapon = weapons [iData.Loadout6 [0]];
				m_Playershooting.m_SecondaryWeapon = weapons [iData.Loadout6 [1]];
			}
		}
		else
		{
			m_Playershooting.m_PrimaryWeapon = weapons [1];
			m_Playershooting.m_SecondaryWeapon = weapons [3];
		}
	}

	public void SavePlayerInventoryData()
	{
		_PlayerInventoryData iData = new _PlayerInventoryData ();

		iData.Pistol_MK1 = weapons[1].isUnlocked;
		iData.Pistol_MK1Ammo = weapons [1].MaxBulletes;

		iData.AssaultRifle_MK1 = weapons[2].isUnlocked;
		iData.AssaultRifle_MK1Ammo = weapons [2].MaxBulletes;

		iData.ShotGun_MK1 = weapons[3].isUnlocked;
		iData.ShotGun_MK1Ammo = weapons [3].MaxBulletes;

		iData.SMG = weapons[4].isUnlocked;
		iData.SMG_Ammo = weapons [4].MaxBulletes;

		iData.MachineGun = weapons[5].isUnlocked;
		iData.MachineGun_Ammo = weapons [5].MaxBulletes;

		iData.Sniper_MK1 = weapons[6].isUnlocked;
		iData.Sniper_MK1Ammo = weapons [6].MaxBulletes;

		iData.Loadout1 = new int[2] {1, 2};
		iData.Loadout2 = new int[2] {1, 3};
		iData.Loadout3 = new int[2] {1, 4};
		iData.Loadout4 = new int[2] {0, 0};
		iData.Loadout5 = new int[2] {0, 0};
		iData.Loadout6 = new int[2] {0, 0};

		iData.MakeDefaultLoadout = _instance.m_SelectedLoadout;

		string saveStatePath = Path.Combine(Application.persistentDataPath, "inventory"+m_PlayerSetup.m_PlayerName+"Info.json");
		File.WriteAllText (saveStatePath, JsonUtility.ToJson (iData, true));

		SaveItOnline ();

		RecalculateWeaponMagazine ();
	}

	int WeaponMagazine(WeaponObject mWeapon)
	{
		if (mWeapon.WeaponIndex == 1)
			return 15;
		else if (mWeapon.WeaponIndex == 2)
			return 40;
		else if (mWeapon.WeaponIndex == 3)
			return 10;
		else if (mWeapon.WeaponIndex == 4)
			return 25;
		else if (mWeapon.WeaponIndex == 5)
			return 200;
		else if (mWeapon.WeaponIndex == 6)
			return 12;
		else
			return 0;
	}
	void SaveItOnline()
	{
		WWWForm form = new WWWForm ();

		form.AddField ("UsernamePost", m_PlayerSetup.m_PlayerName);

		form.AddField ("Pistolmk1Post", weapons[1].isUnlocked.ToString());
		form.AddField ("PistolAmmoPost", weapons [1].MaxBulletes);

		form.AddField ("AssaultRiflemk1Post", weapons[2].isUnlocked.ToString());
		form.AddField ("AssaultRifleAmmoPost", weapons [2].MaxBulletes);

		form.AddField ("ShotGunmk1Post", weapons[3].isUnlocked.ToString());
		form.AddField ("ShotGunAmmoPost", weapons [3].MaxBulletes);

		form.AddField ("SMGPost", weapons[4].isUnlocked.ToString());
		form.AddField ("SMGAmmoPost", weapons [4].MaxBulletes);

		form.AddField ("MachineGunPost", weapons[5].isUnlocked.ToString());
		form.AddField ("MachineGunAmmoPost", weapons [5].MaxBulletes);

		form.AddField ("Snipermk1Post", weapons[6].isUnlocked.ToString());
		form.AddField ("SniperAmmoPost", weapons [6].MaxBulletes);

		form.AddField ("dloadoutPost", "1");

		WWW www = new WWW ("http://databasehandler42.gearhostpreview.com/inc/saveinventorydata.php", form);
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

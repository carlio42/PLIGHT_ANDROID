using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;

public class PlayerSignup : MonoBehaviour
{
	public InputField UsernameInput;
	public InputField PasswordInput;
	public Text SignupText;

	public void Signup()
	{
		StartCoroutine (SignUpIenum());
	}

	IEnumerator SignUpIenum()
	{
		Debug.Log ("Entry");
		playerData data = new playerData ();

		if (UsernameInput.text == string.Empty || PasswordInput.text == string.Empty)
			yield return null;

		WWWForm checkuser = new WWWForm ();
		checkuser.AddField ("UsernamePost", UsernameInput.text);

		WWW www1 = new WWW ("http://databasehandler42.gearhostpreview.com/inc/checkusername.php", checkuser);
		yield return www1;

		if (www1.text == "exists") 
		{
			Debug.Log ("Exists");
			SignupText.text = "USER ALREADY EXISTS !";
			yield return null;
		} 
		else if (www1.text == "none") 
		{
			data.Username = UsernameInput.text;
			data.Password = PasswordInput.text;

			data.PlayerTotalXp = 0;
			data.PlayerXPSliderValue = 0;
			data.PlayerincomingXPSliderValue = 0;

			data.NumberOfCoins = 1000;
			data.NumberOfGems = 250;

			string saveStatePath = Path.Combine (Application.persistentDataPath, "player" + UsernameInput.text + "Info.json");
			File.WriteAllText (saveStatePath, JsonUtility.ToJson (data, true));

			CreateInventoryData ();
			StartCoroutine (StoreinDatabase ());
		}
	}
	void CreateInventoryData()
	{
		PlayerInventoryData iData = new PlayerInventoryData ();

		iData.Pistol_MK1 = true;
		iData.Pistol_MK1Ammo = 750;

		iData.AssaultRifle_MK1 = true;
		iData.AssaultRifle_MK1Ammo = 2000;

		iData.ShotGun_MK1 = false;
		iData.ShotGun_MK1Ammo = 0;

		iData.SMG = false;
		iData.SMG_Ammo = 0;

		iData.MachineGun = false;
		iData.MachineGun_Ammo = 0;

		iData.Sniper_MK1 = false;
		iData.Sniper_MK1Ammo = 0;

		iData.Loadout1 = new int[2] {1, 2};
		iData.Loadout2 = new int[2] {1, 3};
		iData.Loadout3 = new int[2] {1, 4};
		iData.Loadout4 = new int[2] {0, 0};
		iData.Loadout5 = new int[2] {0, 0};
		iData.Loadout6 = new int[2] {0, 0};

		iData.MakeDefaultLoadout = 1;

		string saveStatePath = Path.Combine(Application.persistentDataPath, "inventory"+UsernameInput.text+"Info.json");
		File.WriteAllText (saveStatePath, JsonUtility.ToJson (iData, true));

	}
	IEnumerator StoreinDatabase()
	{
		WWWForm form = new WWWForm ();

		form.AddField ("UsernamePost", UsernameInput.text);
		form.AddField ("PasswordPost", PasswordInput.text);
		form.AddField ("FullnamePost", "");
		form.AddField ("EmailPost", "");
		form.AddField ("CountryPost", "");
		form.AddField ("TotalXPPost", "0");
		form.AddField ("SliderXPPost", "0");
		form.AddField ("IncomingSliderXPPost", "0");
		form.AddField ("TotalCoinsPost", "1000");
		form.AddField ("TotalGemsPost", "250");
		form.AddField ("DOB", "0000-00-00");

		form.AddField ("Pistolmk1Post", "true");
		form.AddField ("PistolAmmoPost", "750");

		form.AddField ("AssaultRiflemk1Post", "true");
		form.AddField ("AssaultRifleAmmoPost", "2000");

		form.AddField ("ShotGunmk1Post", "false");
		form.AddField ("ShotGunAmmoPost", "0");

		form.AddField ("SMGPost", "false");
		form.AddField ("SMGAmmoPost", "0");

		form.AddField ("MachineGunPost", "false");
		form.AddField ("MachineGunAmmoPost", "0");

		form.AddField ("Snipermk1Post", "false");
		form.AddField ("SniperAmmoPost", "0");

		form.AddField ("dloadoutPost", "1");

		WWW www = new WWW ("http://databasehandler42.gearhostpreview.com/inc/signupinc.php", form);
		yield return www;
		SignupText.text = www.text;
	}
}


[Serializable]
class playerData
{
	public string Username;
	public string Password;

	public int PlayerTotalXp;
	public int PlayerXPSliderValue;
	public int PlayerincomingXPSliderValue;

	public int NumberOfCoins;
	public int NumberOfGems;

}

[Serializable]
class PlayerInventoryData
{
	public bool Pistol_MK1 = false;
	public int Pistol_MK1Ammo = 0;

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


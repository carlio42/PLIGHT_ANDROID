using System.Collections;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;
namespace Prototype.NetworkLobby
{
	public class PlayerDataPers : MonoBehaviour 
	{
		public InputField UsernameInput;
		public InputField PasswordInput;
		[SerializeField]
		protected Text EventMessages;
		public Button StartBtn;
		public string[] items;
		public string[] Inventoryitems;

		public int m_PlayerTotalXP;
		public float m_PlayerXPSliderValue;
		public float m_PlayerincomingXPSliderValue;

		public int m_NumberOfCoins;
		public int m_NumberOfGems;

		public int PlayerLoadout;

		void Start()
		{
			LobbyTopPanel.instance = this;
			SavePlayerProgressOnline._instance = this;
			Inventory._instance = this;
			StartBtn.interactable = false;
		}

		public void LoadUserDataLocal()	//Save playerdata into local disk [public : Will Call from "SavePlayerProgressOnline.cs"]
		{
			string saveStatePath = Path.Combine(Application.persistentDataPath, "player"+UsernameInput.text+"Info.json");

			playerData data = new playerData ();
			string dataJSON = File.ReadAllText (saveStatePath);
			data = JsonUtility.FromJson<playerData> (dataJSON);

			m_PlayerTotalXP = data.PlayerTotalXp;
			m_PlayerXPSliderValue = data.PlayerXPSliderValue;
			m_PlayerincomingXPSliderValue = data.PlayerincomingXPSliderValue;

			m_NumberOfCoins = data.NumberOfCoins;
			m_NumberOfGems = data.NumberOfGems;

		}

		public void LoadUserDataDB()		//[public : Will cal at login while pressing login button]
		{
			StartCoroutine (LoadData());
		}

		private IEnumerator LoadData()
		{		
			string saveStatePath = Path.Combine(Application.persistentDataPath, "player"+UsernameInput.text+"Info.json");
			string saveinventoryPath = Path.Combine(Application.persistentDataPath, "inventory"+UsernameInput.text+"Info.json");


			WWWForm form = new WWWForm ();
			form.AddField ("UsernamePost", UsernameInput.text);
			form.AddField ("PasswordPost", PasswordInput.text);

			EventMessages.text = "<color=white>Fetching account...</color>";

			WWW www = new WWW ("http://databasehandler42.gearhostpreview.com/inc/logininc.php", form);
			yield return www;

			if (www.text == "error") 
			{
				EventMessages.text = "<color=red>User not found !</color>";
				yield return null;
			}

			else if (www.text == "done") 
			{
				EventMessages.text = "<color=green>Logged in</color>";

				WWWForm form2 = new WWWForm ();
				form2.AddField ("UsernamePost", UsernameInput.text);

				WWW ItemData = new WWW ("http://databasehandler42.gearhostpreview.com/inc/userdatainc.php", form2);
				yield return ItemData;

				string ItemDataString = ItemData.text;
				items = ItemDataString.Split (';');

				if (File.Exists (saveStatePath)) 
				{
					m_PlayerTotalXP = Int32.Parse(GetDataValue (items[0], "TotalXP:"));//;//data.PlayerTotalXp;
					m_PlayerXPSliderValue = Int32.Parse(GetDataValue (items[0], "SliderXP:"));//data.PlayerXPSliderValue;
					m_PlayerincomingXPSliderValue = Int32.Parse(GetDataValue(items[0], "IncomingXP:"));//data.PlayerincomingXPSliderValue;
					m_NumberOfCoins = Int32.Parse(GetDataValue (items[0], "TotalCoins:"));//data.NumberOfCoins;
					m_NumberOfGems = Int32.Parse(GetDataValue (items[0], "TotalGems:"));//data.NumberOfGems;

					StartCoroutine(CreateUserInventoryData ());
				}
				else 
				{
					CreateUserData ();
					StartCoroutine(CreateUserInventoryData ());
				}
				StartBtn.interactable = true;
			}
		}
		public string GetDataValue(string data, string index)
		{
			string value = data.Substring(data.IndexOf(index)+index.Length);
			if(value.Contains("|"))
			value = value.Remove (value.IndexOf("|"));
			return value;
		}

		public string GetIDataValue(string data, string index)
		{
			string value = data.Substring(data.IndexOf(index)+index.Length);
			if(value.Contains("|"))
				value = value.Remove (value.IndexOf("|"));
			
			return value;
		}

		public void CreateUserData()
		{
			playerData data = new playerData ();

			data.Username = UsernameInput.text;
			data.Password = PasswordInput.text;

			data.PlayerTotalXp = Int32.Parse(GetIDataValue (items[0], "TotalXP:"));
			data.PlayerXPSliderValue = Int32.Parse(GetIDataValue (items[0], "SliderXP:"));
			data.PlayerincomingXPSliderValue = Int32.Parse(GetIDataValue(items[0], "IncomingXP:"));

			data.NumberOfCoins = Int32.Parse(GetIDataValue (items[0], "TotalCoins:"));
			data.NumberOfGems = Int32.Parse(GetIDataValue (items[0], "TotalGems:"));

			string saveStatePath = Path.Combine (Application.persistentDataPath, "player" + UsernameInput.text + "Info.json");
			File.WriteAllText (saveStatePath, JsonUtility.ToJson (data, true));


			m_PlayerTotalXP = data.PlayerTotalXp;
			m_PlayerXPSliderValue = data.PlayerXPSliderValue;
			m_PlayerincomingXPSliderValue = data.PlayerincomingXPSliderValue;

			m_NumberOfCoins = data.NumberOfCoins;
			m_NumberOfGems = data.NumberOfGems;

		}

		public IEnumerator CreateUserInventoryData()
		{
			WWWForm InventoryForm = new WWWForm ();
			InventoryForm.AddField ("UsernamePost", UsernameInput.text);

			WWW InventoryItemData = new WWW ("http://databasehandler42.gearhostpreview.com/inc/userinventorydatainc.php", InventoryForm);
			yield return InventoryItemData;

			string ItemDataString = InventoryItemData.text;
			Inventoryitems = ItemDataString.Split (';');

			PlayerInventoryData iData = new PlayerInventoryData ();

			iData.Pistol_MK1 = Boolean.Parse(GetIDataValue (Inventoryitems[0], "PistolMK1:"));
			iData.Pistol_MK1Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "PistolAmmo:"));

			iData.AssaultRifle_MK1 = Boolean.Parse(GetIDataValue (Inventoryitems[0], "AssaultRifleMK1:"));
			iData.AssaultRifle_MK1Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "AssaultRifleAmmo:"));

			iData.ShotGun_MK1 = Boolean.Parse(GetIDataValue (Inventoryitems[0], "ShotGunMK1:"));
			iData.ShotGun_MK1Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "ShotGunAmmo:"));

			iData.SMG = Boolean.Parse(GetIDataValue (Inventoryitems[0], "SMG:"));
			iData.SMG_Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "SMGAmmo:"));

			iData.MachineGun = Boolean.Parse(GetIDataValue (Inventoryitems[0], "MachineGun:"));
			iData.MachineGun_Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "MachineGunAmmo:"));

			iData.Sniper_MK1 = Boolean.Parse(GetIDataValue (Inventoryitems[0], "SniperMK1:"));
			iData.Sniper_MK1Ammo = Int32.Parse (GetIDataValue(Inventoryitems[0], "SniperAmmo:"));

			iData.Loadout1 = new int[2] {1, 2};
			iData.Loadout2 = new int[2] {1, 3};
			iData.Loadout3 = new int[2] {1, 4};
			iData.Loadout4 = new int[2] {0, 0};
			iData.Loadout5 = new int[2] {0, 0};
			iData.Loadout6 = new int[2] {0, 0};

			iData.MakeDefaultLoadout = Int32.Parse(GetIDataValue (Inventoryitems[0], "DefaultLoadout:"));
			PlayerLoadout = Int32.Parse(GetIDataValue (Inventoryitems[0], "DefaultLoadout:"));

			string saveStatePath = Path.Combine(Application.persistentDataPath, "inventory"+UsernameInput.text+"Info.json");
			File.WriteAllText (saveStatePath, JsonUtility.ToJson (iData, true));

		}
	}

	[Serializable]
	class playerData
	{
		public string Username = string.Empty;
		public string Password = string.Empty;

		public int PlayerTotalXp = 0;
		public float PlayerXPSliderValue = 0;
		public float PlayerincomingXPSliderValue = 0;

		public int NumberOfCoins = 0;
		public int NumberOfGems = 0;

	}

	[Serializable]
	class TutorialData
	{
		public int c = 0;
	}
}
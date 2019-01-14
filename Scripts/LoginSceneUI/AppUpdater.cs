using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//1st: WholeGame
//2nd: Major Components(Database, New inventory, anti-hack Systems)
//3rd: small bugs

public class AppUpdater : MonoBehaviour 
{
	int firstNumber, secondNumber, thirdNumber;	
	private string[] numbers;
	private InternetActivity m_Internet;
	[SerializeField]
	private Text m_CurrentVersion;
	[SerializeField]
	private Text m_NewerVersion;
	[SerializeField]
	private CanvasGroup m_UpdatePanel;

	void Start ()
	{
		m_Internet = GetComponent<InternetActivity> ();
		StartCoroutine(CheckForNewUpdate ());
	}

	IEnumerator CheckForNewUpdate()
	{
		AppVersionPredefineValues appvr = new AppVersionPredefineValues ();
		if (m_Internet.isConnected) 
		{
			WWW AppVersion = new WWW ("http://databasehandler42.gearhostpreview.com/inc/appversion.php");
			yield return AppVersion;

			string ItemDataString = AppVersion.text;
			numbers = ItemDataString.Split (';');

			firstNumber = Int32.Parse (GetDataValue (numbers [0], "Number1:"));
			secondNumber = Int32.Parse (GetDataValue (numbers [0], "Number2:"));
			thirdNumber = Int32.Parse (GetDataValue (numbers [0], "Number3:"));

			if (firstNumber == appvr.Number1 && secondNumber == appvr.Number2 && thirdNumber == appvr.Number3) 
			{
				m_UpdatePanel.gameObject.SetActive (false);
			}
			else 
			{
				m_UpdatePanel.gameObject.SetActive (true);
				m_CurrentVersion.text = "current verion : "+appvr.Number1+"."+appvr.Number2+"."+appvr.Number3;
				m_NewerVersion.text = "newer verion : "+firstNumber+"."+secondNumber+"."+thirdNumber;
			}
		}

		yield return new WaitForSeconds (3);
		Start ();
	}

	public string GetDataValue(string data, string index)
	{
		string value = data.Substring(data.IndexOf(index)+index.Length);
		if(value.Contains("|"))
			value = value.Remove (value.IndexOf("|"));
		return value;
	}

	public void GoToUpdateURL()
	{
		Application.OpenURL ("http://pro42.gearhostpreview.com/Home/download.php");
	}
	public void ExitfromUpdate()
	{
		Application.Quit ();
	}
}

[Serializable]
class AppVersionPredefineValues
{
	public int Number1 = 1;
	public int Number2 = 0;
	public int Number3 = 0;
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

//Here data is syncing between Server - Client
public class PlayerSetup : NetworkBehaviour 
{
	[SyncVar(hook = "OnChangeColor")]
	public int MaterialColorIndex;
	public Material[] Materials;

	[Header("UI")]
	public Text m_PlayerNameText;

	[SyncVar]
	public int m_PlayerNumber;

	[SyncVar]
	public string m_PlayerName;

	[SyncVar]
	public string m_PlayerPassword;

	[SyncVar]
	public int m_PlayerTotalXP;

	[SyncVar]
	public int m_XPSliderProgress;

	[SyncVar]
	public int m_NumberofCoins;

	[SyncVar]
	public int m_NumberofGems;

	[SyncVar]
	public int m_LoadoutNumber;

	[SyncVar]
	public bool m_IsReady = false;

	public GameObject m_PlayerMesh;

	public override void OnStartClient()
	{
		base.OnStartClient ();

		if(!isServer)		//if not hosting , we have the tank to the gameManager for easy access!
		{
			Debug.Log("PlayerSetup");
		
			GameManager.AddPlayer(gameObject, m_PlayerNumber, m_PlayerName, m_PlayerPassword, m_PlayerTotalXP, m_XPSliderProgress, m_NumberofCoins, m_NumberofGems, m_LoadoutNumber);
		}
		m_PlayerNameText.text = m_PlayerName;

	}

	void Start()
	{
		//if (GameManager.GameMode == "TDMM-1")
			//StartCoroutine (SyncColor());
	}
	/*IEnumerator SyncColor()
	{
		yield return new WaitForSeconds (3.0f);
		OnChangeColor (MaterialColorIndex);
	}
	*/
	public override void OnNetworkDestroy()
	{
		GameManager.s_Instance.RemovePlayer (gameObject);
	}

	public void MeshColor(int newMaterial)
	{
		OnChangeColor (newMaterial);
	}
	void OnChangeColor(int newMat)
	{
		MaterialColorIndex = newMat;
		m_PlayerMesh.GetComponent<SkinnedMeshRenderer>().material = Materials[MaterialColorIndex];
	}
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyManager : NetworkBehaviour 
{
	public List<string> InGameText = new List<string> ();
	public Button m_InGameTextPrefab;
	public RectTransform InGameTextPanel;

	public GameObject m_EnemyPrefab;
	float m_SpanTime = 3.0f;
	int m_NumberofEnemies;
	int m_EnemyCountDown;
	public Transform[] m_SpawnPoints;	
	[HideInInspector]
	public int m_currentEnemyCount;
	//public Image m_ShowText;

	void Start()
	{
		EnemyHealth.m_EnemyManager = this;
		m_currentEnemyCount = m_NumberofEnemies;
	}
	public void ResetEnemies()
	{
		m_EnemyCountDown = 0;
	}
	public void SetEnemyNumber(int value)
	{
		m_NumberofEnemies = value;
	}
	public override void OnStartServer()
	{
		SpawnEnemy ();
	}

	public void SpawnEnemy()
	{
		InvokeRepeating ("Spawn", m_SpanTime, m_SpanTime);
	}

	void Update()
	{
		m_currentEnemyCount = GameObject.FindGameObjectsWithTag ("Enemy").Length;
	}

	void Spawn()
	{
		if (m_NumberofEnemies == m_EnemyCountDown) 
			return;
		
		m_EnemyCountDown++;

		int m_SpawnIndex = Random.Range (0,m_SpawnPoints.Length);

		GameObject m_Enemy = Instantiate (m_EnemyPrefab, m_SpawnPoints [m_SpawnIndex].position, m_SpawnPoints [m_SpawnIndex].rotation);
		NetworkServer.Spawn (m_Enemy);
			
	}

	[Server]
	public void ShowOnServer(string _playerName)
	{
		InGameText.Add (_playerName);
		RpcDispMessage ();
	}
	[ClientRpc]
	void RpcDispMessage()
	{
		Disp ();
	}

	void Disp()
	{
		foreach (string names in InGameText) 
		{
			Button ob = Instantiate (m_InGameTextPrefab) as Button;
			ob.GetComponentInChildren<Text> ().text = names + "killed an enemy";
			ob.transform.SetParent (InGameTextPanel, false);
			StartCoroutine(RemoveNames (names,ob));

		}
		//m_ShowText.gameObject.SetActive (true);
		//m_ShowText.GetComponentInChildren<Text> ().text = _playerName + " killed 1 enemy";
		//yield return new WaitForSeconds (2.0f);
		//m_ShowText.gameObject.SetActive (false);
	}
	IEnumerator RemoveNames(string names, Button b)
	{
		yield return new WaitForSeconds (5.0f);
		InGameText.Remove (names);
		Destroy (b.gameObject);
		//foreach (GameObject t in InGameTextPanel)
		//	Destroy (t.gameObject);

	}

	public void SetDefault()
	{
		m_NumberofEnemies = 0;
		m_EnemyCountDown = 0;
	}
}

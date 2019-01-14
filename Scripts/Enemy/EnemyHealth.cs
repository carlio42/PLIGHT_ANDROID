using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyHealth : NetworkBehaviour 
{
	public const float m_MaxHealth = 100;

	[SyncVar(hook = "OnChangeHealth")]
	public float m_CurrentHealth = m_MaxHealth;
	Collider m_EnemyCollider;
	bool isDead;
	public bool isSinking;
	public static EnemyManager m_EnemyManager;

	private Animator m_anim;

	void Awake()
	{
		m_EnemyCollider = GetComponent<CapsuleCollider> ();
		m_anim = GetComponentInChildren<Animator> ();
		//m_anim.SetBool ("isDead", false);
	}
	void Update()
	{
		if (isSinking)
			transform.Translate (-Vector3.up * 3.5f *Time.deltaTime);
	}

	public void TakeDamageToEnemy(float amount, Vector3 hitPoint, GameObject m_Player)
	{
		if (!isServer)
			return;
		
		if (isDead)
			return;

		m_CurrentHealth -= amount;
		RpcTakingDamageAnimation ();
		m_Player.GetComponent<PlayerAchievementCount> ().LocalPlayerScoreCount (1);

		if (m_CurrentHealth <= 1) 
		{
			m_Player.GetComponent<PlayerAchievementCount> ().LocalPlayerScoreCount (50);
			m_Player.GetComponent<PlayerAchievementCount> ().LocalPlayerKiilCount (1);
			Death (m_Player);		
		}
	}

	void OnChangeHealth(float _currentHealth)
	{
		m_CurrentHealth = _currentHealth;	
	}

	void Death(GameObject _player)
	{
		
		isDead = true;
		m_EnemyCollider.isTrigger = true;
		GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
		RpcDeadAnimation ();
		//GetComponent<Rigidbody> ().isKinematic = true;
		isSinking = true;
		m_EnemyManager.ShowOnServer (_player.GetComponent<PlayerSetup>().m_PlayerName);
		Destroy (gameObject, 4f);
	}

	[ClientRpc]
	void RpcTakingDamageAnimation()
	{
		m_anim.SetTrigger ("TakeDamage");
	}

	[ClientRpc]
	void RpcDeadAnimation()
	{
		int i = 1;
		i = Random.Range (1, 3);
		m_anim.SetBool ("Die"+i, true);
	}
}

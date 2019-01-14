using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class EnemyMovement : NetworkBehaviour
{
	GameObject[] m_Players;
	[SyncVar (hook = "ChangeFollowPlayer")]
	public Transform m_FollowPlayerTransform;
	NavMeshAgent agent;

	Animator m_anim;
	int i,j;

	void Start()
	{
		agent = GetComponent<NavMeshAgent> ();
		m_anim = GetComponent<Animator> ();
		StartCoroutine (Animate());
	}

	IEnumerator Animate()
	{
		i = Random.Range (1,3);
		j = Random.Range (-1, 2);
		yield return new WaitForSeconds (0.2f);
		m_anim.SetBool ("Idle"+i, true);
	}
	void FixedUpdate()
	{
		m_Players = GameObject.FindGameObjectsWithTag ("Player");
		SelectTarget ();
	}

	void SelectTarget()
	{
		foreach (GameObject players in m_Players) 
		{
			if (Vector3.Distance (agent.transform.position, players.transform.position) <= 40.0f) 
			{
				m_FollowPlayerTransform = players.transform;
				ChangeFollowPlayer (m_FollowPlayerTransform);
				agent.SetDestination (m_FollowPlayerTransform.position);
				EnemyAnimation (false, true);
			}

			else 
			{
				m_FollowPlayerTransform = null;
				EnemyAnimation (true, false);
			}
		}
	}

	void ChangeFollowPlayer(Transform newTransform)
	{
		m_FollowPlayerTransform = newTransform;
	}

	void EnemyAnimation(bool isIdle, bool isRun)
	{
		m_anim.SetBool ("Idle"+i, isIdle);
		m_anim.SetBool("IsWalking", isRun);
	}

}
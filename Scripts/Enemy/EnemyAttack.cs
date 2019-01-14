using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyAttack : NetworkBehaviour 
{
	float TimeBetweenAttack = 0.5f;
	public float timer;
	EnemyHealth m_EnemyHealth;

	public GameObject Raypos;
	public GameObject RangeRayPos;

	public Ray shootRay = new Ray();
	public Ray RangeRay = new Ray();

	public RaycastHit shootHit;
	public RaycastHit RangeHit;

	Animator m_anim;
	private bool playerInRange;


	void Awake()
	{
		m_EnemyHealth = GetComponent<EnemyHealth> ();
		m_anim = GetComponentInChildren<Animator> ();
	}

	void FixedUpdate()
	{
		timer += Time.deltaTime;

		RangeRay.origin = RangeRayPos.transform.position;
		RangeRay.direction = RangeRayPos.transform.forward;

		if (Physics.Raycast (RangeRay, out RangeHit, 2f)) 
		{
			GameObject ChitPlayer = RangeHit.collider.gameObject;
			if (ChitPlayer.layer == 14)
				playerInRange = true;
			else
				playerInRange = false;
		}

		if (timer >= TimeBetweenAttack && playerInRange && m_EnemyHealth.m_CurrentHealth > 5)
			Attack ();
	}

	void Attack()
	{
		RpcEnemyAttack ();

		shootRay.origin = Raypos.transform.position;
		shootRay.direction = Raypos.transform.forward;

		if (Physics.Raycast (shootRay, out shootHit, 2f)) 
		{
			GameObject hitPlayer = shootHit.collider.gameObject;
			CmdShotPlayer (hitPlayer, hitPlayer.layer, shootHit.point);
		}
	}

	void CmdShotPlayer(GameObject hitPlayer, int LayerMaskNo, Vector3 hitPoint)
	{
		if (hitPlayer.layer == 14)
			hitPlayer.GetComponent<PlayerHealth> ().TakeDamageToPlayer (3, hitPlayer);
	}

	void RpcEnemyAttack()
	{
		timer = 0;
		m_anim.SetTrigger ("IsAttack");
	}


}

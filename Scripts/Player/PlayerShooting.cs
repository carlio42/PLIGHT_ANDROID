using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerShooting : NetworkBehaviour 
{
	private PlayerHealth m_Health;
	[SyncVar(hook="OnChangeLayerMask")]
	int m_LayerMask;

	public Button PrWeaponUI;
	public Button SecWeaponUI;

	WeaponManager m_WeaponManager;
	private Transform m_Cam;
	private Vector3 m_CrossHairForward;

	[HideInInspector]
	public int score;

	public WeaponObject m_PrimaryWeapon;
	public WeaponObject m_SecondaryWeapon;
	public WeaponObject m_currentWeapon;

	public GameObject LaserSpawn;
	public LineRenderer GunLaser;
	public ParticleSystem GunParticles;

	public Ray shootRay = new Ray();
	public Ray AutoAimRay = new Ray ();
	public RaycastHit shootHit;
	public RaycastHit AimHit;
	private Animator m_Animator;

	private float timer = 0f;

	public override void OnStartClient()
	{
		GunLaser.enabled = false;
		//StartCoroutine (SetWeapon());
	}

	void Start()
	{
		m_Health = GetComponent<PlayerHealth> ();
		m_WeaponManager = GetComponent<WeaponManager> ();
		m_Animator = GetComponentInChildren<Animator> ();
		m_Cam = Camera.main.transform;

		if (GameManager.GameMode == "TDMM-1")
			StartCoroutine (SyncLayerMask());
	}

	IEnumerator SyncLayerMask()
	{
		yield return new WaitForSeconds (4.0f);
		OnChangeLayerMask (m_LayerMask);
	}
	public void SetupShooting()
	{
		StartCoroutine (SetWeapon());
	}
	IEnumerator SetWeapon()
	{
		yield return new WaitForSeconds (2.5f);
		if (!isLocalPlayer)
			yield return null;
		if (m_PrimaryWeapon == null) 
			m_currentWeapon = m_SecondaryWeapon;
		else
			m_currentWeapon = m_PrimaryWeapon;	

		PrWeaponUI.GetComponent<Image>().sprite = m_PrimaryWeapon.WeaponImage;
		SecWeaponUI.GetComponent<Image>().sprite = m_SecondaryWeapon.WeaponImage;

	}

	void ChangeWeapon()
	{
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) 
		{
			if (m_currentWeapon == m_PrimaryWeapon)
				m_currentWeapon = m_SecondaryWeapon;
			else if(m_currentWeapon == m_SecondaryWeapon)
				m_currentWeapon = m_PrimaryWeapon;
		}

		else if(Input.GetAxis ("Mouse ScrollWheel") < 0)
		{
			if (m_currentWeapon == m_PrimaryWeapon)
				m_currentWeapon = m_SecondaryWeapon;
			else if(m_currentWeapon == m_SecondaryWeapon)
				m_currentWeapon = m_PrimaryWeapon;
		}
	}

	void CrossHairRotation()
	{
		m_CrossHairForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 1, 1)).normalized;

		Quaternion WantedRotation = Quaternion.LookRotation (m_CrossHairForward);
		LaserSpawn.transform.rotation = Quaternion.Slerp (LaserSpawn.transform.rotation, WantedRotation, 100 * Time.deltaTime);
	}

	[ClientCallback]
	void Update()
	{
		if (m_Health.m_IsPlayerDead)
			return;
		//Calculate the time for gun shooting

		CrossHairRotation ();

		timer += Time.deltaTime;
		if (m_currentWeapon == null)
			return;
		
		ChangeWeapon ();
		
		if (!isLocalPlayer)
			return;
		AutoTarget ();

		if (Input.GetKey (KeyCode.Space) && timer >= m_currentWeapon.FireRate) 
		{
			if (!m_Health.m_IsPlayerDead)
				Fire ();
		}

		if (timer >= m_currentWeapon.FireRate * m_currentWeapon.EffectDisplayTime) 
		{
			if (!m_Health.m_IsPlayerDead)
				StopFire ();
		}
	}

	void AutoTarget()
	{
		AutoAimRay.origin = LaserSpawn.transform.position;
		AutoAimRay.direction = LaserSpawn.transform.forward;
		if (Physics.Raycast (AutoAimRay, out AimHit, 100.0f)) 
		{
			GameObject HitObj = AimHit.collider.gameObject;

			if (HitObj.layer == 11 && timer >= m_currentWeapon.FireRate) 
			{
				if (!m_Health.m_IsPlayerDead)
					Fire ();
			}

			if (HitObj.layer == 15 && timer >= m_currentWeapon.FireRate) 
			{
				if (!m_Health.m_IsPlayerDead)
					Fire ();
			}
			if (HitObj.layer == 16 && timer >= m_currentWeapon.FireRate) 
			{
				if (!m_Health.m_IsPlayerDead)
					Fire ();
			}

			if (timer >= m_currentWeapon.FireRate * m_currentWeapon.EffectDisplayTime) 
			{
				if (!m_Health.m_IsPlayerDead)
					StopFire ();
			}
		}
	}

	void Fire()
	{
		if (m_currentWeapon.Magazine <= 0) 
		{
			Debug.Log ("Reload in PlayerShooting");
			m_WeaponManager.Reload (m_currentWeapon);

			return;
		}
		GunLaser.SetPosition (0, LaserSpawn.transform.position);
		shootRay.origin = LaserSpawn.transform.position;
		shootRay.direction = LaserSpawn.transform.forward;

		if (Physics.Raycast (shootRay, out shootHit, m_currentWeapon.range)) 
		{
			GameObject hitPlayer = shootHit.collider.gameObject;
			CmdShotToSomeone (hitPlayer, hitPlayer.layer, shootHit.point);
			GunLaser.SetPosition (1, shootHit.point);
		}
		else
			GunLaser.SetPosition (1, shootRay.origin + shootRay.direction * m_currentWeapon.range);

		GunLaser.enabled = true;
		CmdShowLaser ();
		m_currentWeapon.Magazine--;

		timer = 0f;
	}

	[Command]
	void CmdShotToSomeone(GameObject hitPlayer, int LayerMaskNo, Vector3 hitPoint)
	{
		if (LayerMaskNo == 9)
			hitPlayer.GetComponent<PlayerHealth> ().TakeDamageToPlayer (m_currentWeapon.damage, this.gameObject);
		else if (LayerMaskNo == 11)
			hitPlayer.GetComponent<EnemyHealth> ().TakeDamageToEnemy (m_currentWeapon.damage * 1.0f, hitPoint, this.gameObject);
		else if (LayerMaskNo == 15) 
		{
			if (this.gameObject.layer == 16)
				hitPlayer.GetComponent<PlayerHealth> ().TakeDamageToPlayer ((m_currentWeapon.damage * 1.0f), this.gameObject);
			else
				return;
		}
		else if (LayerMaskNo == 16) 
		{
			if (this.gameObject.layer == 15)
				hitPlayer.GetComponent<PlayerHealth> ().TakeDamageToPlayer (m_currentWeapon.damage * 1.0f, this.gameObject);
			else
				return;
		}
	}

	[Command]
	void CmdShowLaser()
	{
		RpcShowLaser ();
	}

	[ClientRpc]
	void RpcShowLaser()
	{
		if (isLocalPlayer)
			return;
		ShowLaser ();
	}

	void ShowLaser()
	{
		m_Animator.SetTrigger ("IsShoot");
		GunLaser.SetPosition (0, LaserSpawn.transform.position);
		shootRay.origin = LaserSpawn.transform.position;
		shootRay.direction = LaserSpawn.transform.forward;

		if (Physics.Raycast (shootRay, out shootHit, m_currentWeapon.range)) 
		{
			GunLaser.SetPosition (1, shootHit.point);
		}

		else
			GunLaser.SetPosition (1, shootRay.origin + shootRay.direction * m_currentWeapon.range);

		GunLaser.enabled = true;
	}

	void StopFire()
	{
		StopLaser ();
		CmdStopLaser ();
	}

	void StopLaser()
	{
		GunLaser.enabled = false;
	}

	[Command]
	void CmdStopLaser()
	{
		RpcStopLaser ();
	}

	[ClientRpc]
	void RpcStopLaser()
	{
		if (isLocalPlayer)
			return;
		StopLaser ();
	}

	public void OnClickPrWeapon()
	{
		if (m_PrimaryWeapon != null)
			m_currentWeapon = m_PrimaryWeapon;
		SecWeaponUI.GetComponent<Image> ().color = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 130.0f/255.0f);
		PrWeaponUI.GetComponent<Image> ().color = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 240.0f/255.0f);

	}
	public void OnClickSecWeapon()
	{
		if (m_SecondaryWeapon != null)
			m_currentWeapon = m_SecondaryWeapon;
		PrWeaponUI.GetComponent<Image> ().color = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 130.0f/255.0f);
		SecWeaponUI.GetComponent<Image> ().color = new Color (255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 240.0f/255.0f);

	}


	public void SetDefaults(int layermask)
	{
		OnChangeLayerMask (layermask);
	}
	void OnChangeLayerMask(int layermask)
	{
		m_LayerMask = layermask;
		gameObject.layer = m_LayerMask;
	}
}
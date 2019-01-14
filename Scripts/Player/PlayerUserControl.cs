using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUserControl : NetworkBehaviour
{
	public static VirtualJoystickNew Joystick;
	private PlayerHealth m_Health;

	private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera

	private int TurnSpeed = 10;
	private float MoveSpeed = 3.75f;
	[HideInInspector]
	public Rigidbody m_Rigidbody;
	private Animator m_Animator;
	public NetworkClient nClient;
	public AudioClip[] m_FootStepSounds;
	private AudioSource m_AudioSource;
	int latency;

	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;

	void Start()
	{
		m_Health = GetComponent<PlayerHealth> ();

		m_Rigidbody = GetComponent<Rigidbody> ();
		m_Animator = GetComponentInChildren<Animator> ();
		m_AudioSource = GetComponent<AudioSource> ();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

		nClient = GameObject.Find ("AndroidUICanvas").GetComponent<NetworkManager> ().client;
		m_Cam = Camera.main.transform;

		m_Animator.SetBool ("IsIdle", true);
	}

	private void FixedUpdate()
	{
		if (!isLocalPlayer)
			return;
		if (m_Health.m_IsPlayerDead)
			return;
		
		HandleAirBorneMovement ();

		Vector3 MoveDir = Vector3.zero;
		MoveDir.x = Joystick.Horizontal();
		MoveDir.z = Joystick.Vertical();

		m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;

		if (MoveDir.magnitude > 1)
			MoveDir.Normalize ();

		Quaternion WantedRotation = Quaternion.LookRotation (m_CamForward);
		transform.rotation = Quaternion.Slerp (transform.rotation, WantedRotation, TurnSpeed * Time.deltaTime);

		transform.Translate (Vector3.right * MoveDir.x * Time.deltaTime * MoveSpeed);
		transform.Translate (Vector3.forward * MoveDir.z * Time.deltaTime * MoveSpeed);

		if (MoveDir.magnitude > 0)
		{
			UpdateAnimator (false, true);
			FootStepSound (false);
		}

		else 
		{
			UpdateAnimator (true, false);
			FootStepSound (true);
		}
	}

	void HandleAirBorneMovement()
	{
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);
	}

	void UpdateAnimator (bool idle, bool run)
	{
		m_Animator.SetBool ("IsIdle", idle);
		m_Animator.SetBool ("IsRun", run);
	}

	public void SetDeafults()
	{
		m_Rigidbody.velocity = Vector3.zero;
		m_Rigidbody.angularVelocity = Vector3.zero;
	}

	void FootStepSound(bool active)
	{
		m_AudioSource.mute = active;
	}
}
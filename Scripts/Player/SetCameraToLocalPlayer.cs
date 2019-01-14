using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace MyNetwork
{
	public class SetCameraToLocalPlayer : NetworkBehaviour 
	{
		static public SetCameraToLocalPlayer s_ingleton;
		private Transform MyTransform;
		private NetworkClient nClient;
		public Text LatencyText;

		void Start()
		{
			s_ingleton = this;
			nClient = GameObject.Find ("AndroidUICanvas").GetComponent<NetworkManager> ().client;
		}
		void Update()
		{
			if(isLocalPlayer)
				ShowLatency ();
		}
		void ShowLatency()
		{
			LatencyText.text = "Ping : "+nClient.GetRTT ()+" ms";
		}

		public override void OnStartLocalPlayer()
		{
			GetNetIdentity ();
		}

		private void Awake()
		{
			MyTransform = this.transform;
		}

		[Client]
		void GetNetIdentity()
		{
			GameObject.FindWithTag("FreeLookCamera").GetComponent<FreeLookCam> ().m_Target =  MyTransform;
		}

		[Client]
		public void ChangeCameraTarget(Transform KilledPlayer)
		{
			GameObject.FindWithTag("FreeLookCamera").GetComponent<FreeLookCam> ().m_Target =  KilledPlayer;
		}

	}

}
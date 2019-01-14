using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace MyNetwork
{
	public class PlayerID : NetworkBehaviour 
	{
		[SyncVar]
		public string PlayerUniqueIdentity;
		private NetworkInstanceId PlayerNetID;
		private Transform MyTransform;

		public override void OnStartLocalPlayer()
		{
			GetNetIdentity ();
			SetIndentity ();
		}

		private void Awake()
		{
			MyTransform = this.transform;
		}
		
		private void Update()
		{
			if (MyTransform.name == "" || MyTransform.name == "Player(Clone)") 
			{
				SetIndentity ();
			}
		}
		
		[Client]
		void GetNetIdentity()
		{
			PlayerNetID = GetComponent<NetworkIdentity> ().netId;
			CmdTellServerMyIdentity (MakeUniqueIdentity());
			GameObject.FindWithTag("FreeLookCamera").GetComponent<FreeLookCam> ().m_Target =  MyTransform;
		}
		
		void SetIndentity()
		{
			if (!isLocalPlayer)
				MyTransform.name = PlayerUniqueIdentity;
			else
				MyTransform.name = MakeUniqueIdentity();
		}
		string MakeUniqueIdentity()
		{
			string UniqueName = "Player"+ PlayerNetID.ToString();
			return UniqueName;
		}
		[Command]
		void CmdTellServerMyIdentity(string name)
		{
			PlayerUniqueIdentity = name;
		}
	}

}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class PlayerAchievementCount : NetworkBehaviour 
{
	
	[SyncVar(hook = "OnScoreChange")]
	public float score;
	[SyncVar(hook = "OnKillChange")]
	public float kill;
	[SyncVar(hook = "OnDeathChange")]
	public float death;
	static public ScoreBoard m_ScoreBoard;

	//-------------------------------KILL COUNT--------------------------------//
	void OnScoreChange(float newValue)
	{
		score = newValue;	
	}

	void OnDeathChange(float newValue)
	{
		death = newValue;
	}

	void OnKillChange(float newValue)
	{
		kill = newValue;
	}

	[Server]
	public void LocalPlayerKiilCount(float value)
	{
		RpcKCount (value);
	}
	[ClientRpc]
	void RpcKCount(float value)
	{
		kill += value;
		OnKillChange (kill);
	}
		
	//------------------------------------------------------------------------//

	//-------------------------------SCORE COUNT--------------------------------//
	[Server]
	public void LocalPlayerScoreCount(float value)
	{
		RpcSCount (value);
	}
	[ClientRpc]
	void RpcSCount(float value)
	{
		score += value;
		OnScoreChange (score);
	}
	//------------------------------------------------------------------------//

	//-------------------------------DEATH COUNT--------------------------------//
	[Server]
	public void LocalPlayerDeathCount(float value)
	{
		RpcDCount (value);
	}
	[ClientRpc]
	void RpcDCount(float value)
	{
		death += value;
		OnDeathChange (death);
	}
	//------------------------------------------------------------------------//
}
	
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerLobbyHook : Prototype.NetworkLobby.LobbyHook 
{
	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
	{
		Debug.Log("LobbyHook");
		if (lobbyPlayer == null)
			return;

		Prototype.NetworkLobby.LobbyPlayer lp = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer> ();
		Prototype.NetworkLobby.LobbyManager lm = GetComponent<Prototype.NetworkLobby.LobbyManager> ();


		if (lp != null) 
		{
			GameManager.AddPlayer (gamePlayer, lp.slot, lp.nameInput.text, lp.playerPassword, lp.playerTotalXP, lp.XPsliderProgress, lp.numberofCoins, lp.numberofGems, lp.loadoutNumber);

			GameManager.GameMode = lm.playScene;
		}

	}
	
}

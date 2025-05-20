using System;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamMainMenu : MonoBehaviour
{
    public static string HostAdressKey = "HostAdress";
    public static string HostAdress = "";
    public MainMenu mainMenu;
    public GameObject save;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    void Start()
    {
        if (!SteamManager.Initialized) return;
        Debug.Log("Steam is initialized");
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.Log("Callback");
        if(NetworkServer.active) return;

        Debug.Log("Entered lobby : " + SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name"));

        HostAdress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);

        mainMenu.SteamJoinGame(save);
    }
}
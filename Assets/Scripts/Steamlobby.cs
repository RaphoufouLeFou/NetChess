using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;

public class Steamlobby : MonoBehaviour
{
    public NetworkManager networkManager;
    public NetworkManagement networkManagement;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    public static string HostAdressKey = "HostAdress";
    public ulong CurrentLobbyId;

    public void HostLobby()
    {
        if (!SteamManager.Initialized) return;

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        Debug.Log("Created lobby");
    }

    private void Start()
    {
        if (!SteamManager.Initialized) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        Debug.Log("Lobby created successfully");
        CurrentLobbyId = callback.m_ulSteamIDLobby;
        networkManagement.StartHostGame();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName() + "'s game.");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining lobby");
        if (NetworkServer.active)
        {
            return;
        }

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;
        Debug.Log("Entered lobby : " + SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name"));
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey);
        networkManagement.StartClientGame(hostAddress);
    }

    public void InviteFriend()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(CurrentLobbyId));
    }
    
    void Update()
    {
        // invite player with "I" key
        if (Input.GetKeyDown(KeyCode.I))
        {
            InviteFriend();
        }
    }
}
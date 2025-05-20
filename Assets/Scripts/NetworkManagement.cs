using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Mirror;
using kcp2k;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagement : MonoBehaviour
{

    public NetworkManager manager;
    public KcpTransport transport;
    public Steamlobby steamlobby;
    void Start()
    {

    }

    public void Awake()
    {

        if (true)
        {
            steamlobby.HostLobby();
            StartHostGame();
        }

        else StartClientGame(SteamMainMenu.HostAdress);

        /*
        manager.GetComponent<KcpTransport>().Port = (ushort)SuperGlobals.Uri.Port;
        manager.networkAddress = SuperGlobals.Uri.Host;

        if (SuperGlobals.IsHost) StartHostGame();
        else StartClientGame(SuperGlobals.Uri);
        */
        if (!SteamManager.Initialized) return;
        SteamMatchmaking.SetLobbyData(new CSteamID(SuperGlobals.callback.m_ulSteamIDLobby), Steamlobby.HostAdressKey, SteamUser.GetSteamID().ToString());

    }

    // Update is called once per frame
    void Update()
    {

    }
    
        public void StartHostGame()
    {
        manager.StartHost();
    }

    private void StartClientGame(Uri uri)
    {
        manager.StartClient(uri);
    }

    public void StartClientGame(string host)
    {
        manager.networkAddress = host;
        Debug.Log("host : " + host);
        manager.StartClient();
    }
}

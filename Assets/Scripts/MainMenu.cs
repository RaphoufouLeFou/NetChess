//#define IS_LIGHTWEIGHT_BUILD

using System;
using System.IO;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenu : MonoBehaviour
{
    [Header("Name of the maine scene to load")]
    public string mainSceneName;

    [Header("GameObjects")]
    public GameObject superGlobals;
    private string _ipAddress = "localhost";
    private string _port = "7777";

    private void Start()
    {
        // send SuperGlobals to main scene
        DontDestroyOnLoad(superGlobals);
        SuperGlobals.CleanUp();
    }

    public void JoinGame()
    {
        StartGame("", true, false, true);
    }
    public void NewGameMulti() => NewGame(true);

    private void NewGame(bool multi)
    {
        StartGame("", true, false, false);
    }

    public void SteamJoinGame(GameObject save)
    {
        string saveName = save.GetComponent<TMP_InputField>().text;
        Debug.Log("from steam join game");
        StartGame(saveName, true, false, true);
    }

    public void SteamStartGame(GameObject save)
    {
        string saveName = save.GetComponent<TMP_InputField>().text;
        Debug.Log("from start game");
        StartGame("", true, false, false );
    }

    private void StartGame(string saveName, bool multi, bool newSave, bool isClient)
    {

        SuperGlobals.IsMultiplayerGame = multi;
        SuperGlobals.IsHost = !isClient;
        SuperGlobals.SaveName = "";
        SuperGlobals.Uri = new Uri($"kcp://{_ipAddress}:{_port}");

        SuperGlobals.EditorMode = false;
        SuperGlobals.PlayerName = "";
        SceneManager.LoadScene(mainSceneName);
    }
}

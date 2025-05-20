using System;
using Steamworks;
using UnityEngine;

public class SuperGlobals : MonoBehaviour
{
    // data that gets sent through scenes
    public static bool EditorMode = true; // set to false if started from main menu

    public static LobbyCreated_t callback;

    // deprecated once game is loaded
    public static string SaveName;

    public static string PlayerName;
    //public static Dictionary<string, uint> PlayersNames = new ();

    // network data
    public static bool IsMultiplayerGame;
    public static bool IsHost;
    public static Uri Uri;

    public static void CleanUp()
    {
        // reset cross-scene things, set up back to menu
        EditorMode = false;
        SaveName = "";
        IsMultiplayerGame = false;
        IsHost = false;

        //PlayersNames.Clear();
    }
}

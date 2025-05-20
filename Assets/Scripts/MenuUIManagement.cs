using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum MenuName
{
    Main,
    NewGame,
    LoadGame,
    JoinGame
}

// handles opening specific menus and menu animations
public class MenuUIManagement : MonoBehaviour
{
    [Header("Parents")]
    public RectTransform mainParent;
    public RectTransform newGameParent;
    public RectTransform loadGameParent;
    public RectTransform joinGameParent;

    public RectTransform backButtonParent;
    public RectTransform usernameParent;

    [Header("Main camera")]
    public GameObject cameraObj;

    [Header("Scripts")]
    public MainMenu mainMenu;

    [Header("Cutscene-related")]
    public VideoPlayer videoPlayer;
    public GameObject videoTexture;
    public GameObject planet;
    public Image blackCover;
    public GameObject discordButton;

    [Header("Music")]
    public AudioSource mainMusic;
    public AudioSource submenuMusic;

    // animation: first show the necessary components, then execute the animation, then hide the old ones

    // animations and music fading management
    private const float AnimTimeNormal = 0.5f;
    private const float AnimTimeSlow = 2;
    private const float AnimTimeCutscene = 2;

    private readonly float[] _prevStates = new float[11];
    private readonly float[] _currentStates = new float[11];
    private float _prevTime;
    // cutscene utilities
    public static float PlayingCutsceneNext;
    private float _cutsceneStart;

    // used to know when exactly to start the initial music fadein, as there could be lag before that
    private bool _isInit;

    // will be longer in certain places
    private float _currentAnimTime;

    private MenuName _menu;
    private bool ShowUsername => _menu is MenuName.NewGame or MenuName.LoadGame or MenuName.JoinGame;

    private void Start()
    {
        // prevent sound glitches at the start
        mainMusic.volume = 0;
        submenuMusic.volume = 0;
        mainMusic.Play();
        submenuMusic.Play();
        blackCover.gameObject.SetActive(false);

        videoTexture.SetActive(false);
    }

    private void GoToMenu(MenuName menu, bool firstTime = false)
    {
        if (PlayingCutsceneNext != 0) return; // don't do anything else at the same time

        _menu = menu;
        ShowOnscreen();

        if (!firstTime)
            for (int i = 0; i < _prevStates.Length; i++)
                _prevStates[i] = _currentStates[i];

        _currentStates[0] = menu == MenuName.Main ? 0 : -1;
        _currentStates[1] = menu == MenuName.NewGame ? 0 : 1;
        _currentStates[2] = menu == MenuName.LoadGame ? 0 : 1;
        _currentStates[3] = menu == MenuName.JoinGame ? 0 : 1;
        _currentStates[4] = menu == MenuName.Main ? 0.5f : 0;
        _currentStates[5] = ShowUsername ? 0 : 0.5f;

        _currentStates[6] = menu == MenuName.Main ? 0 : 20;
        _currentStates[7] = menu == MenuName.Main ? 0.3f : -0.4f;
        _currentStates[8] = menu == MenuName.Main ? 0 : 0.2f;

        _currentStates[9] = menu == MenuName.Main ? 1 : 0;
        _currentStates[10] = menu == MenuName.Main ? 0 : 1;

        if (firstTime)
        {
            // no animation at the start
            for (int i = 0; i < 9; i++) _prevStates[i] = _currentStates[i];

            // music fadein
            _prevStates[9] = 0;
            _prevStates[10] = 0;

            _currentAnimTime = AnimTimeSlow;
        }

        else _currentAnimTime = AnimTimeNormal;

        _prevTime = Time.time;
    }

    private void ShowOnscreen()
    {
        if (_menu == MenuName.Main) mainParent.gameObject.SetActive(true);
        if (_menu == MenuName.NewGame) newGameParent.gameObject.SetActive(true);
        if (_menu == MenuName.LoadGame) loadGameParent.gameObject.SetActive(true);
        if (_menu == MenuName.JoinGame) joinGameParent.gameObject.SetActive(true);
        if (_menu != MenuName.Main) backButtonParent.gameObject.SetActive(true);
        if (ShowUsername) usernameParent.gameObject.SetActive(true);
    }

    private void HideOffscreen()
    {
        if (_menu != MenuName.Main) mainParent.gameObject.SetActive(false);
        if (_menu != MenuName.NewGame) newGameParent.gameObject.SetActive(false);
        if (_menu != MenuName.LoadGame) loadGameParent.gameObject.SetActive(false);
        if (_menu != MenuName.JoinGame) joinGameParent.gameObject.SetActive(false);
        if (_menu == MenuName.Main) backButtonParent.gameObject.SetActive(false);
        if (!ShowUsername) usernameParent.gameObject.SetActive(false);
    }

    public void NewGame() => GoToMenu(MenuName.NewGame);
    public void Back() => GoToMenu(MenuName.Main);
    public void LoadGame() => GoToMenu(MenuName.LoadGame);
    public void JoinGame() => GoToMenu(MenuName.JoinGame);
    public void CloseGame() => Application.Quit();

    public void ScheduleCutscene()
    {
        PlayingCutsceneNext = Time.time + AnimTimeCutscene;
        Color col = blackCover.color;
        col.a = 0;
        blackCover.color = col;
        
        blackCover.gameObject.SetActive(true);
    }

    public void DiscordButton() => Application.OpenURL("https://discord.gg/6x3mCXQu7e");
    
    public void SkipCutscene()
    {
        PlayingCutsceneNext = 0;
        _cutsceneStart = 0;
    }

    private void Update()
    {
        if (!_isInit)
        {
            GoToMenu(MenuName.Main, true);
            _isInit = true;
        }
    }
}

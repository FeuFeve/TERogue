using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour {
    // Buttons to be selected when the associated screen is shown
    public GameObject mainPlayButton;
    public GameObject mainHighscoresButton;
    public GameObject mainSettingsButton;
    public GameObject mainCreditsButton;

    public GameObject singleplayerButton;
    public GameObject multiplayerButton;

    public GameObject singleplayerLetter1Button;
    public GameObject singleplayerLetter2Button;
    public GameObject singleplayerLetter3Button;

    public GameObject multiplayerPl1Letter1Button;
    public GameObject multiplayerPl1Letter2Button;
    public GameObject multiplayerPl1Letter3Button;
    public GameObject multiplayerPl2Letter1Button;
    public GameObject multiplayerPl2Letter2Button;
    public GameObject multiplayerPl2Letter3Button;

    public GameObject highscoresToMultiplayerScoresButton;
    public GameObject highscoresToSingleplayerScoresButton;
    public GameObject highscoresBackToMainMenuButton;

    public GameObject settingsGraphicsButton;
    public GameObject settingsMusicsVolumeButton;
    public GameObject settingsSoundsVolumeButton;

    private GameObject selectedGameObject;
    private bool lockGameObjectSelection = false;
    private float lockChangeUntil;

    // Main menu animator
    public static Animator animator;

    // Player names
    public static char[] player1Pseudo;
    public static char[] player2Pseudo;

    // Highscores display
    private bool displaySingleplayerScores = true; // true = Singleplayer scores, false = Multiplayer scores, reset to true when back

    private bool isInCredits = false;


    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Start() {
        animator = GetComponent<Animator>();

        selectedGameObject = mainPlayButton;

        player1Pseudo = new char[] { 'A', 'A', 'A' };
        player2Pseudo = new char[] { 'A', 'A', 'A' };
        lockChangeUntil = Time.time;
        
        Settings settings = Settings.instance;
        settingsGraphicsButton.GetComponentInChildren<TextMeshProUGUI>().text = "" + settings.quality;
        settingsMusicsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.musicsVolume + "%";
        settingsSoundsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.soundsVolume + "%";
    }

    public void Update() {
        if (!lockGameObjectSelection && Time.time > lockChangeUntil) {
            if (isInCredits) {
                HandleCreditsExiting();
            }
            else {
                // Menu navigation between buttons
                HandleNavigation();

                // Changes letters in name selection, or settings
                HandleLetterChanges(singleplayerLetter1Button, 1, 1, multiplayerPl1Letter1Button);
                HandleLetterChanges(singleplayerLetter2Button, 1, 2, multiplayerPl1Letter2Button);
                HandleLetterChanges(singleplayerLetter3Button, 1, 3, multiplayerPl1Letter3Button);

                HandleLetterChanges(multiplayerPl1Letter1Button, 1, 1, singleplayerLetter1Button);
                HandleLetterChanges(multiplayerPl1Letter2Button, 1, 2, singleplayerLetter2Button);
                HandleLetterChanges(multiplayerPl1Letter3Button, 1, 3, singleplayerLetter3Button);
                HandleLetterChanges(multiplayerPl2Letter1Button, 2, 1, null);
                HandleLetterChanges(multiplayerPl2Letter2Button, 2, 2, null);
                HandleLetterChanges(multiplayerPl2Letter3Button, 2, 3, null);

                // Changes in the settings window
                HandleSettingsChanges();
            }
        }
    }

    private void HandleNavigation() {
        Selectable toSelect = null;
        int axisCount = 0;

        if (Input.GetAxis("Up") == 1) {
            toSelect = selectedGameObject.GetComponent<Button>().navigation.selectOnUp;
            axisCount++;
        }
        if (Input.GetAxis("Down") == 1) {
            toSelect = selectedGameObject.GetComponent<Button>().navigation.selectOnDown;
            axisCount++;
        }
        if (Input.GetAxis("Right") == 1) {
            toSelect = selectedGameObject.GetComponent<Button>().navigation.selectOnRight;
            axisCount++;
        }
        if (Input.GetAxis("Left") == 1) {
            toSelect = selectedGameObject.GetComponent<Button>().navigation.selectOnLeft;
            axisCount++;
        }

        if (toSelect != null && axisCount == 1) {
            selectedGameObject = toSelect.gameObject;
            lockChangeUntil = Time.time + 0.2f;
            AudioManager.instance.Play(SFX.MenuHover);
        }
        
        select(selectedGameObject);
    }

    private void HandleLetterChanges(GameObject button, int playerNum, int letterNum, GameObject linkedButton) {
        if (selectedGameObject == button) {
            int change = 0;
            if (Input.GetAxis("FireUp") == 1 || Input.GetAxis("P2FireUp") == 1)
                change--;
            else if (Input.GetAxis("FireDown") == 1 || Input.GetAxis("P2FireDown") == 1)
                change++;
            else
                return;

            if (change == 0)
                return;
            
            char[] playerPseudo;
            if (playerNum == 1) playerPseudo = player1Pseudo;
            else playerPseudo = player2Pseudo;

            char newLetter = playerPseudo[letterNum - 1];
            if (change == 1) newLetter++;
            else newLetter--;

            if (newLetter > 'Z') newLetter = 'A';
            else if (newLetter < 'A') newLetter = 'Z';

            playerPseudo[letterNum - 1] = newLetter;

            button.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = newLetter.ToString();
            if (linkedButton != null)
                linkedButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = newLetter.ToString();

            lockChangeUntil = Time.time + 0.1f;
            AudioManager.instance.Play(SFX.MenuHover);
        }
    }

    private void HandleSettingsChanges() {
        Settings settings = Settings.instance;

        // Graphics
        if (selectedGameObject == settingsGraphicsButton) {
            if (Input.GetAxis("Right") == 1 && Input.GetAxis("Left") == 0) {
                settings.IncreaseQuality();
                settingsGraphicsButton.GetComponentInChildren<TextMeshProUGUI>().text = "" + settings.quality;
                lockChangeUntil = Time.time + 0.5f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
            else if (Input.GetAxis("Left") == 1 && Input.GetAxis("Right") == 0) {
                settings.DecreaseQuality();
                settingsGraphicsButton.GetComponentInChildren<TextMeshProUGUI>().text = "" + settings.quality;
                lockChangeUntil = Time.time + 0.5f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
        }

        // Musics
        else if (selectedGameObject == settingsMusicsVolumeButton) {
            if (Input.GetAxis("Right") == 1 && Input.GetAxis("Left") == 0) {
                settings.IncreaseMusicsVolume();
                settingsMusicsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.musicsVolume + "%";
                lockChangeUntil = Time.time + 0.2f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
            else if (Input.GetAxis("Left") == 1 && Input.GetAxis("Right") == 0) {
                settings.DecreaseMusicsVolume();
                settingsMusicsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.musicsVolume + "%";
                lockChangeUntil = Time.time + 0.2f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
        }

        // Sounds
        else if (selectedGameObject == settingsSoundsVolumeButton) {
            if (Input.GetAxis("Right") == 1 && Input.GetAxis("Left") == 0) {
                settings.IncreaseSoundsVolume();
                settingsSoundsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.soundsVolume + "%";
                lockChangeUntil = Time.time + 0.2f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
            else if (Input.GetAxis("Left") == 1 && Input.GetAxis("Right") == 0) {
                settings.DecreaseSoundsVolume();
                settingsSoundsVolumeButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.soundsVolume + "%";
                lockChangeUntil = Time.time + 0.2f;
                AudioManager.instance.Play(SFX.MenuHover);
            }
        }
    }

    private void HandleCreditsExiting() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("MainMenu to Credits") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) { // MainMenu to Credits animation has finished
            if (Input.anyKey) { // Press any key to go back to the main menu
                CreditsToMainMenu();
            }
        }
    }

    private void unselect() {
        lockGameObjectSelection = true;
        EventSystem.current.SetSelectedGameObject(null);
        selectedGameObject = null;
    }

    private void select(GameObject gameObject) {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
        selectedGameObject = gameObject;
        lockGameObjectSelection = false;
    }

    public void MainMenuToGameMode() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 1.5f);
        animator.Play("MainMenu to Game Mode", 0, 0);

        select(singleplayerButton);
    }

    public void GameModeToMainMenu() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", -1.5f);
        animator.Play("MainMenu to Game Mode", 0, 1);

        select(mainPlayButton);
    }

    public void GameModeToSinglePlayer() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 1.5f);
        animator.Play("Game Mode to Singleplayer", 0, 0);

        select(singleplayerLetter1Button);
    }

    public void SinglePlayerToGameMode() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", -1.5f);
        animator.Play("Game Mode to Singleplayer", 0, 1);

        select(singleplayerButton);
    }

    public void GameModeToMultiPlayer() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 1.5f);
        animator.Play("Game Mode to Multiplayer", 0, 0);

        select(multiplayerPl1Letter1Button);
    }

    public void MultiPlayerToGameMode() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", -1.5f);
        animator.Play("Game Mode to Multiplayer", 0, 1);

        select(multiplayerButton);
    }

    public void StartSingleplayerGame() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        GameData.isMultiplayer = false;
        GameData.player1Name = new string(player1Pseudo);

        SceneSwitcher.Singleton.StartCoroutine("AsyncStartGame");
    }

    public void StartMultiplayerGame() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        GameData.isMultiplayer = true;
        GameData.player1Name = new string(player1Pseudo);
        GameData.player2Name = new string(player2Pseudo);

        SceneSwitcher.Singleton.StartCoroutine("AsyncStartGame");
    }

    // Called by SceneSwitcher
    public static void SingleplayerToStartGame() {
        EventSystem.current.SetSelectedGameObject(null);

        animator.SetFloat("Speed", 1.5f);
        animator.Play("Singleplayer to Start Game", 0, 0);
    }

    // Called by SceneSwitcher
    public static void MultiPlayerToStartGame() {
        GameData.isMultiplayer = true;
        EventSystem.current.SetSelectedGameObject(null);

        animator.SetFloat("Speed", 1.5f);
        animator.Play("Multiplayer to Start Game", 0, 0);
    }

    public void MainMenuToHighscores() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 1.5f);
        animator.Play("MainMenu to Highscores", 0, 0);

        select(highscoresToMultiplayerScoresButton);
    }

    public void HighscoresToMainMenu() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        if (displaySingleplayerScores) {
            animator.SetFloat("Speed", -1.5f);
            animator.Play("MainMenu to Highscores", 0, 1);
        }
        else {
            animator.SetFloat("Speed", 1.5f);
            animator.Play("Highscores Multiplayer to MainMenu", 0, 0);
            displaySingleplayerScores = true;

            Navigation navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = highscoresToMultiplayerScoresButton.GetComponent<Button>();
            highscoresBackToMainMenuButton.GetComponent<Button>().navigation = navigation;
        }

        select(mainHighscoresButton);
    }

    public void HighscoresSingleplayerToMultiplayer() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 2.0f);
        animator.Play("Highscores Singleplayer to Multiplayer", 0, 0);

        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = highscoresToSingleplayerScoresButton.GetComponent<Button>();
        highscoresBackToMainMenuButton.GetComponent<Button>().navigation = navigation;

        displaySingleplayerScores = false;
        select(highscoresToSingleplayerScoresButton);
    }

    public void HighscoresMultiplayerToSingleplayer() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", -2.0f);
        animator.Play("Highscores Singleplayer to Multiplayer", 0, 1);

        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = highscoresToMultiplayerScoresButton.GetComponent<Button>();
        highscoresBackToMainMenuButton.GetComponent<Button>().navigation = navigation;

        displaySingleplayerScores = true;
        select(highscoresToMultiplayerScoresButton);
    }

    public void MainMenuToSettings() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", 1.5f);
        animator.Play("MainMenu to Settings", 0, 0);

        select(settingsGraphicsButton);
    }

    public void SettingsToMainMenu() {
        AudioManager.instance.Play(SFX.MenuSelect);
        unselect();

        animator.SetFloat("Speed", -1.5f);
        animator.Play("MainMenu to Settings", 0, 1);

        select(mainSettingsButton);
    }

    public void MainMenuToCredits() {
        AudioManager.instance.Play(SFX.MenuSelect);
        AudioManager.instance.SetMusicTheme(MusicTheme.Credits);
        unselect();

        animator.SetFloat("Speed", 1.0f);
        animator.Play("MainMenu to Credits", 0, 0);

        isInCredits = true;
        select(null);
    }

    public void CreditsToMainMenu() {
        isInCredits = false;
        AudioManager.instance.Play(SFX.MenuSelect);
        AudioManager.instance.SetMusicTheme(MusicTheme.MainMenu);
        unselect();

        animator.SetFloat("Speed", 1.0f);
        animator.Play("Credits to MainMenu", 0, 0);

        select(mainCreditsButton);
    }

    public void Quit() {
        Debug.Log("Exiting the game.");
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSwitcher : MonoBehaviour {
    public static SceneSwitcher Singleton;
    public static bool alreadyLoading = false;
    public Animator animator;

    [HideInInspector]
    public GameObject floor;

    private void Start() {
        Singleton = this;
    }

    IEnumerator AsyncSwitchScene() {
        if (!alreadyLoading) {
            alreadyLoading = true;

            // Change the background music context
            if (GameData.roomType == "MobRoom")
                FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.NormalFloors);
            if (GameData.roomType == "MerchantRoom")
                FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.Merchant);
            if (GameData.roomType == "BossRoom")
                FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.Boss);

            animator.SetFloat("Speed", 5.0f); // (2) 5/1 = 5* speed, base speed = 1s
            animator.Play("Fade Out", 0, 0);
            yield return new WaitForSeconds(0.20f); // 1/0.20 = 5* speed, base speed = 1s

            UpdateFloorUI();

            yield return SceneManager.UnloadSceneAsync(GameData.predRoomType);
            yield return new WaitForSeconds(0.2f);

            yield return SceneManager.LoadSceneAsync(GameData.roomType, LoadSceneMode.Additive);
            yield return new WaitForSeconds(0.2f);

            animator.SetFloat("Speed", 5.0f); // (2) 5/1 = 5* speed, base speed = 1s
            animator.Play("Fade In", 0, 0);
            yield return new WaitForSeconds(0.20f); // 1/0.20 = 5* speed, base speed = 1s

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameData.roomType));
            alreadyLoading = false;
        }
    }

    // MainMenu to EntryRoom scene
    IEnumerator AsyncStartGame() {
        if (!alreadyLoading) {
            alreadyLoading = true;

            FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.NormalFloors);

            // Main menu animation
            if (GameData.isMultiplayer)
                MainMenu.MultiPlayerToStartGame();
            else
                MainMenu.SingleplayerToStartGame();
            yield return new WaitForSeconds(1.0f);

            // Load the game scenes
            yield return SceneManager.LoadSceneAsync(GameData.roomType, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("Persistant", LoadSceneMode.Additive);
            UpdateFloorUI();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameData.roomType));

            SceneManager.UnloadSceneAsync("MainMenu");
            Stats.Init();
            alreadyLoading = false;
        }
    }

    // Game scene to MainMenu
    IEnumerator AsyncBackToMainMenu() {
        if (!alreadyLoading) {
            alreadyLoading = true;

            FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.MainMenu);

            // Game scene fade out animation
            GameOver.animator.SetFloat("Speed", 1.0f); // (1)
            GameOver.animator.Play("Fade Out", 0, 0);
            yield return new WaitForSeconds(1.0f); // (2) = inverse of (1)

            // Load the MainMenu and unload the other scenes
            SceneManager.LoadSceneAsync("MainMenu");
            GameData.Init();

            alreadyLoading = false;
        }
    }

    // Reload game scene (when clicking on "Retry")
    IEnumerator AsyncRetry() {
        if (!alreadyLoading) {
            alreadyLoading = true;

            // Game scene fade out animation
            GameOver.animator.SetFloat("Speed", 1.0f); // (1)
            GameOver.animator.Play("Fade Out", 0, 0);
            yield return new WaitForSeconds(1.0f); // (2) = inverse of (1)

            // Unload and reload the scenes to reset all of their components
            yield return SceneManager.UnloadSceneAsync(GameData.roomType);
            GameData.Init();
            yield return SceneManager.LoadSceneAsync(GameData.roomType, LoadSceneMode.Additive);

            SceneManager.UnloadSceneAsync("Persistant");
            SceneManager.LoadSceneAsync("Persistant", LoadSceneMode.Additive);
            UpdateFloorUI();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameData.roomType));
            Stats.Init();

            FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.NormalFloors);

            alreadyLoading = false;
        }
    }

    private void UpdateFloorUI() {
        if (floor == null)
            floor = GameObject.Find("Main Information/Floor");
        floor.GetComponent<TextMeshProUGUI>().text = "Floor " + GameData.level;
    }
}

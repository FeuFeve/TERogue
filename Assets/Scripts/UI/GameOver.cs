using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

/**
 * In game, use:
 * - `FindObjectOfType<GameOver>().Trigger();` to make the GameOver Tab appear
 */
public class GameOver : MonoBehaviour {

    public TextMeshProUGUI statsUI;

    public GameObject retryButton;
    public GameObject mainMenuButton;

    private GameObject selectedGameObject;
    private bool lockGameObjectSelection = false;
    private float lockChangeUntil;

    // Persistant scene canvas animator
    public static Animator animator;


    void Start() {
        animator = transform.parent.GetComponent<Animator>();
    }

    void Update() {
        if (!lockGameObjectSelection && Time.time > lockChangeUntil) {
            // Navigation between buttons
            HandleNavigation();
        }
    }

    public void Trigger() {
        // Prevent a bug causing the player to sometimes stay alive after GameOver
        foreach (GameObject player in Player.playerList) {
            player.GetComponent<Player>().Die();
        }

        Stats.StopTimer();
        statsUI.text = GetStats();
        FadeInGameOverTab();
    }

    private string GetStats() {
        return "Score: " + FindObjectOfType<ScoreManager>().GetScore() + "\n"
            + "Kills: " + Stats.killsCount + "\n"
            + "Game time: " + Stats.GetGameTime();
    }
    
    private void FadeInGameOverTab() {
        unselect();

        animator.SetFloat("Speed", 1.0f);
        animator.Play("GameOver", 0, 0);

        select(retryButton);
    }

    public void Retry() {
        SceneSwitcher.Singleton.StartCoroutine("AsyncRetry");
    }

    public void BackToMainMenu() {
        SceneSwitcher.Singleton.StartCoroutine("AsyncBackToMainMenu");
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

    private void HandleNavigation() {
        if (selectedGameObject == null)
            return;

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
            select(selectedGameObject);
        }
        else {
            select(selectedGameObject);
        }
    }
}

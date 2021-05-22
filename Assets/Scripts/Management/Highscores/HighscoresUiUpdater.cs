using TMPro;
using UnityEngine;

/*
 * Only used in the MainMenu scene, to update the singleplayer/multiplayer Highscore tables
 */
public class HighscoresUiUpdater : MonoBehaviour {

    public GameObject singleplayerHighscoresTableUI;
    public GameObject multiplayerHighscoresTableUI;
    
    public static bool dataHasChanged = false;


    private void Start() {
        UpdateHighscoresUI();
    }

    private void Update() {
        if (dataHasChanged) {
            UpdateHighscoresUI();
            dataHasChanged = false;
        }
    }

    private void UpdateHighscoresUI() {
        HighscoresManager.LoadHtIfNecessary();
        UpdateTableUI(singleplayerHighscoresTableUI, true);
        UpdateTableUI(multiplayerHighscoresTableUI, false);
    }

    private void UpdateTableUI(GameObject tableUI, bool isSingleplayerTable) {
        for (int i = 0; i < HighscoresManager.MAX_HIGHSCORES; i++) {
            Highscore highscore;
            if (isSingleplayerTable)
                highscore = HighscoresManager.GetSingleplayerScore(i);
            else
                highscore = HighscoresManager.GetMultiplayerScore(i);

            TextMeshProUGUI left = tableUI.transform.GetChild(i + 1).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI right = tableUI.transform.GetChild(i + 1).GetChild(1).GetComponent<TextMeshProUGUI>();

            if (highscore != null) {
                left.text = highscore.pseudo;
                right.text = "" + highscore.score;
            }
            else {
                left.text = "???";
                right.text = "???";
            }
        }
    }
}

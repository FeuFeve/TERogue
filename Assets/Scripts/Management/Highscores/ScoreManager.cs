using UnityEngine;
using TMPro;

/**
 * In game, use:
 * - `FindObjectOfType<ScoreManager>().AddWithMultiplier(int);` to add score (multipliers will be applied to this value)
 * - `FindObjectOfType<ScoreManager>().RemoveWithMultiplier(int);` to remove score (multipliers will be applied to this value)
 * - `FindObjectOfType<ScoreManager>().ApplyPlayerDeathPenality();` to remove score from player death
 */
public class ScoreManager : MonoBehaviour {

    private bool hasSaved = false;

    // Current score related
    private int score = 0;
    public float baseScoreMultiplier = 1.0f;
    public float scoreMultiplierPerFloor = 0.1f;

    public int scorePenalityAtDeath = 500;
    public int penalityAddonPerFloor = 50;

    // UI elements
    public TextMeshProUGUI scoreUI;


    // Start is called before the first frame update
    private void Start() {
        UpdateScoreUI();
    }

    private void Update() {
        if (Player.playerList.Count == 0 && !hasSaved) { // All players are dead: GameOver
            HighscoresManager.SaveCurrentScore(score);
            hasSaved = true;
        }
    }

    public void AddWithMultiplier(int scoreValue) {
        score += Mathf.FloorToInt(scoreValue * (baseScoreMultiplier + GameData.level * scoreMultiplierPerFloor));
        UpdateScoreUI();
    }

    public void RemoveWithMultiplier(int scoreValue) {
        score -= Mathf.FloorToInt(scoreValue * baseScoreMultiplier * GameData.level * scoreMultiplierPerFloor);
        if (score < 0)
            score = 0;
        UpdateScoreUI();
    }

    public void ApplyPlayerDeathPenality() {
        score -= scorePenalityAtDeath + penalityAddonPerFloor * GameData.level;
        if (score < 0)
            score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI() {
        scoreUI.text = "Score " + score;
    }

    public int GetScore() {
        return score;
    }
}

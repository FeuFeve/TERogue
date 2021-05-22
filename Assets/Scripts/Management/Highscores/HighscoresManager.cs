using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class HighscoresManager {

    public static readonly int MAX_HIGHSCORES = 10;

    private static HighscoreTables HT;
    private static string filePath = Application.persistentDataPath + "/highscores.bin";
    
    
    public static void InsertSingleplayerScore(Highscore score) {
        LoadHtIfNecessary();
        if (HT.InsertSingleplayerScore(score)) {
            SaveHighscores();
        }
    }
    
    public static void InsertMultiplayerScore(Highscore score) {
        LoadHtIfNecessary();
        if (HT.InsertMultiplayerScore(score)) {
            SaveHighscores();
        }
    }

    public static void LoadHtIfNecessary() {
        if (HT == null) {
            LoadHighscores();
        }
    }

    public static void SaveCurrentScore(int score) {
        string pseudo;
        if (GameData.isMultiplayer) {
            pseudo = GameData.player1Name + "/" + GameData.player2Name;
            InsertMultiplayerScore(new Highscore(pseudo, score));
        }
        else {
            pseudo = GameData.player1Name;
            InsertSingleplayerScore(new Highscore(pseudo, score));
        }
    }

    private static void SaveHighscores() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(stream, HT);
        stream.Close();
        HighscoresUiUpdater.dataHasChanged = true;
    }

    private static void LoadHighscores() {
        if (File.Exists(filePath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);
            HT = formatter.Deserialize(stream) as HighscoreTables;
            stream.Close();
        }
        else { // When first launching the game, create the file with empty highscore slots
            HT = new HighscoreTables();
            SaveHighscores();
        }
    }

    public static Highscore GetSingleplayerScore(int index) {
        return HT.GetSingleplayerScore(index);
    }

    public static Highscore GetMultiplayerScore(int index) {
        return HT.GetMultiplayerScore(index);
    }
}

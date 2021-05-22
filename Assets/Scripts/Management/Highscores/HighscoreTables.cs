[System.Serializable]
public class HighscoreTables {

    private Highscore[] singleplayerHighscores = new Highscore[HighscoresManager.MAX_HIGHSCORES];
    private Highscore[] multiplayerHighscores = new Highscore[HighscoresManager.MAX_HIGHSCORES];


    /*
     * Try to insert a score into the singleplayer highscores table: returns true if inserted, false otherwise
     */
    public bool InsertSingleplayerScore(Highscore score) {
        return InsertScore(score, singleplayerHighscores);
    }

    /*
     * Try to insert a score into the multiplayer highscores table: returns true if inserted, false otherwise
     */
    public bool InsertMultiplayerScore(Highscore score) {
        return InsertScore(score, multiplayerHighscores);
    }

    private bool InsertScore(Highscore score, Highscore[] table) {
        bool added = false;
        Highscore copy = null;

        for (int i = 0; i < table.Length; i++) {
            if (table[i] == null) { // The slot is empty
                if (added) {
                    table[i] = copy;
                }
                else {
                    table[i] = score;
                }
                return true;
            }
            else {
                if (added) {
                    Highscore tempCopy = new Highscore(table[i]);
                    table[i] = new Highscore(copy);
                    copy = tempCopy;
                }
                else {
                    if (table[i].score < score.score) {
                        copy = new Highscore(table[i]);
                        table[i] = score;
                        added = true;
                    }
                }
            }
        }

        return added;
    }

    public Highscore GetSingleplayerScore(int index) {
        return singleplayerHighscores[index];
    }

    public Highscore GetMultiplayerScore(int index) {
        return multiplayerHighscores[index];
    }

    // TODO: remove ToString()
    public override string ToString() {
        string toReturn = "### Singleplayer scores:";
        foreach (Highscore highscore in singleplayerHighscores) {
            if (highscore != null)
                toReturn += "\n" + highscore;
            else
                toReturn += "\n??? - ???";
        }

        toReturn += "\n### Multiplayer scores:";
        foreach (Highscore highscore in multiplayerHighscores) {
            if (highscore != null)
                toReturn += "\n" + highscore;
            else
                toReturn += "\n??? - ???";
        }

        return toReturn;
    }
}
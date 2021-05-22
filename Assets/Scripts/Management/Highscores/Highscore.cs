[System.Serializable]
public class Highscore {

    public string pseudo;
    public int score;


    public Highscore(string pseudo, int score) {
        this.pseudo = pseudo;
        this.score = score;
    }

    // Copy constructor
    public Highscore(Highscore highscore) {
        pseudo = highscore.pseudo;
        score = highscore.score;
    }

    public override string ToString() {
        return pseudo + " - " + score;
    }
}
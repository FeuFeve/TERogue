using UnityEngine;

public abstract class Powerup : MonoBehaviour {

    public string title = "";
    public string description = "";

    public void Start () {

    }

    public void OnTriggerEnter2D(Collider2D collision) {
        AudioManager.instance.Play(SFX.PowerUp);
    }

    public void PrintInfo () {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        InfoPrinter.Singleton.Print(title, description);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Gold : MonoBehaviour {
    public int amount;
    
    // Update is called once per frame
    void Update() {
        foreach(GameObject p in Player.playerList) {
            GetComponent<Rigidbody2D>().AddForce(p.transform.position - transform.position);
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == "Player") {
            foreach (GameObject p in Player.playerList) {
                p.GetComponent<Player>().gold += amount;
            }
            AudioManager.instance.Play(SFX.PlayerPickUpCoin);
            Destroy(gameObject);
        }
    }
}

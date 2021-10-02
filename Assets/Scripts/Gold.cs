using UnityEngine;

public class Gold : MonoBehaviour {
    public int amount;
    
    // Update is called once per frame
    void Update() {
        foreach(GameObject p in Player.playerList) {
            Vector3 direction = (p.transform.position - transform.position).normalized;
            float distance = (p.transform.position - transform.position).magnitude;
            float force = (1f / distance) * 30;
            force = Mathf.Min(200, force);
            GetComponent<Rigidbody2D>().AddForce(direction * force);    
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

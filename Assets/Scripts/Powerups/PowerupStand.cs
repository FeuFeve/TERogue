using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupStand : MonoBehaviour
{
    [System.Serializable]
    public class PowerupPriceClass {
        public GameObject powerup;
        public int price;
    }

    public List<PowerupPriceClass> powerups;

    private GameObject powerup;
    private int price;
    private TextMeshPro textMeshPro;

    // Start is called before the first frame update
    void Start () {
        PowerupPriceClass ppc = powerups[Random.Range(0, powerups.Count)];
        //instantiating
        powerup = Instantiate(ppc.powerup, transform);
        price = ppc.price;
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.text = "" + price;
        //position
        powerup.transform.position = transform.position + new Vector3(0, 1, 0);
        //disable the powerup
        powerup.GetComponent<Powerup>().enabled = false;
        powerup.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {
            if (collision.gameObject.GetComponent<Player>().gold >= price) {
                //buy
                collision.gameObject.GetComponent<Player>().gold -= price;
                //give
                powerup.transform.position += new Vector3(0, -1f, 0);
                powerup.GetComponent<Powerup>().enabled = true;
                powerup.GetComponent<Collider2D>().enabled = true;
                //disable shop
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
                Destroy(textMeshPro.gameObject);
            }
        }
    }
}

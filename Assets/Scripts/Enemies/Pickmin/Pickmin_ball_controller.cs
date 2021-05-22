using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickmin_ball_controller : MonoBehaviour
{
    public GameObject toSummon;
    public float damages = 12f;
    private bool summoned = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Character>().Damage(damages, HpChangesType.normalDamages);
        }
        if (!summoned)
        {
            Instantiate(toSummon, transform.position, Quaternion.identity);
            Destroy(gameObject);
            summoned = true;
        }

    }
}

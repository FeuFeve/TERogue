using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOURDIN_controller : MonoBehaviour
{
    public float contact_damage = 20f;
    protected float cacTickTime = 0.8f;
    protected float lastTickTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Character chara = collision.gameObject.GetComponent<Character>();
            if (chara != null && Time.time >= lastTickTime + cacTickTime)
            {
                chara.Damage(contact_damage, HpChangesType.normalDamages); // TODO: modifier en fonction du type de dégâts infligés
                this.lastTickTime = Time.time;
            }
        }
    }
}

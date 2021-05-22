using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilSpar_IAcontroller : IA_controller
{
    public GameObject explosion;

    public override void Start()
    {
        base.Start();
        this.attack_state = new LilSpark_Attack_state(this);
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(collision.collider.tag =="Player") Die();
        if (explosion != null)
        {
            GameObject e = Instantiate(explosion, transform.parent);
            e.transform.position = transform.position;
        }
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log("State : " + this.currentState);
    }
}

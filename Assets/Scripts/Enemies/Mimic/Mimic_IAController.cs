using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_IAController : IA_controller
{
    private bool opened;
    public override void Start()
    {
        base.Start();
        this.wandering_state = new Mimic_Wandering_State(this);
        this.attack_state = new Mimic_Attack_State(this);
        this.opened = false;
        changeState(wandering_state);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.tag == "Player")
        {
            foreach (GameObject player in Player.playerList)
            {
                if (player.activeInHierarchy && !targets.Contains(player.transform)) targets.Add(player.transform);
            }
            if (!opened) animator.SetTrigger("Open");
        }

    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        if (!opened)
        {
            changeState(tracking_state);
            opened = true;
        }
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log("State : " + currentState);
    }

    public override void Damage(float value, HpChangesType damageType)
    {
        base.Damage(value, damageType);
        if (!opened) animator.SetTrigger("Open");
    }
}

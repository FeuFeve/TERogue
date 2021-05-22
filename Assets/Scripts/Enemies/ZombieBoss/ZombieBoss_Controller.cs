using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBoss_Controller : IA_controller
{
    public GameObject toThrow;
    public override void Start()
    {
        base.Start();
        this.attack_state = new ZombieBoss_Attack_State(this, toThrow);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_IAController : IA_controller
{
    public override void Start()
    {
        base.Start();
        this.attack_state = new Bat_Attack_State(this);
    }

    public override void Update()
    {
        base.Update();
    }
}

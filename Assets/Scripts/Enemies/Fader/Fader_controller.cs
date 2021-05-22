using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader_controller : IA_controller
{
    public override void Start()
    {
        base.Start();
        this.attack_state = new Fader_Attack_State(this);
    }
    public override void Update()
    {
        base.Update();
    }
}

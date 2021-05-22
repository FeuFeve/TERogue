using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COD_Player_controller : IA_controller
{

    public LineRenderer indicator;
    public override void Start()
    {
        base.Start();
        this.attack_state = new COD_Player_Attack_State(this, indicator);
    }
}

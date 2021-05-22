using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss_controller : IA_controller
{
    public override void Start()
    {
        base.Start();
        this.attack_state = new DemonBoss_attackState(this);
    }
}

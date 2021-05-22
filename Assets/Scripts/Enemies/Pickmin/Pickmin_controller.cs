using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickmin_controller : IA_controller
{
    public override void Start()
    {
        base.Start();
        BasicAttackState attack = new BasicAttackState(this);
        attack.AddAction(new BasicRangeAttack(attack, 3f));
        this.attack_state = attack;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader_Attack_State : BasicAttackState
{
    public Fader_Attack_State(IA_controller controller) : base(controller)
    {
        this.actions.Add(new Fader_teleport_Attack(this, 5f));
        this.actions.Add(new BasicRangeAttack(this, 2.5f));
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
    }
}

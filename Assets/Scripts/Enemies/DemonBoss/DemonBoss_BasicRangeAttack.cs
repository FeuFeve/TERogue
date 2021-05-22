using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss_BasicRangeAttack : Action
{
    bool attacking;

    public DemonBoss_BasicRangeAttack(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State state)
    {
        base.Act(state);
        
        if(!attacking)
        {
            End();
        }
       
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        this.attacking = true;
        this.caller.controller.Move(Vector2.zero);
        this.caller.controller.animator.SetTrigger("Attacking");
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        Vector2 dir = caller.controller.targetDir();
        caller.controller.Shoot(dir);
        this.attacking = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilSpark_Attack_state : BasicAttackState
{
    public LilSpark_Attack_state(IA_controller controller) : base(controller)
    {
        BasicRushAttack rush_attack = new BasicRushAttack(this, 2f, false);
        rush_attack.setRushSpeed(8f);
        this.actions.Add(rush_attack);
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
    }

    public override void animationTriggerIsCalled(IA_controller controller)
    {
        base.animationTriggerIsCalled(controller);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
    }
}

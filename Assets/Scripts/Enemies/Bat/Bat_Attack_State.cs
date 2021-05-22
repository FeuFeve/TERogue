using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_Attack_State : BasicAttackState
{
    public Bat_Attack_State(IA_controller controller) : base(controller)
    {
        BasicRushAttack rush_attack = new BasicRushAttack(this, 3f, false);
        rush_attack.setRushSpeed(8f);
        this.actions.Add(rush_attack);
        this.actions.Add(new BasicRangeAttack(this, 1.5f));
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

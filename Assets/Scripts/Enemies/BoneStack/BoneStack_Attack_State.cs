using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneStack_Attack_State : BasicAttackState
{
    public BoneStack_Attack_State(IA_controller controller) : base(controller)
    {
        this.actions.Add(new BasicRangeAttack(this, 1f));
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

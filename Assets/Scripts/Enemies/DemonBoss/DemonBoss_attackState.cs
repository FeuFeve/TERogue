using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss_attackState : BasicAttackState
{
    public DemonBoss_attackState(IA_controller controller) : base(controller)
    {
        this.actions.Add(new DemonBoss_BasicRangeAttack(this, 1f));
        this.actions.Add(new DemonBoss_rush(this, 5f));
        this.actions.Add(new DemonBoss_eat_Attack(this, 3f));
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
        //Debug.Log(currentAction.GetType().Name);
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

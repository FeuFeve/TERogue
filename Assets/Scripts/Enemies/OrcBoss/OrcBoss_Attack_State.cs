using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcBoss_Attack_State : BasicAttackState
{
    GameObject zoneIndicator;
    public OrcBoss_Attack_State(IA_controller controller, GameObject zoneIndicator) : base(controller)
    {
        this.zoneIndicator = zoneIndicator;
        this.actions.Add(new OrcBoss_Swipe_Attack(this, 2f, zoneIndicator));
        this.actions.Add(new OrcBoss_Throw_Gourdin(this, 7f));
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

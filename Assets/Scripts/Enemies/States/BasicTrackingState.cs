using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTrackingState : State
{

    bool targetIsValid;
    bool targetInRange;
    bool targetInVision;

    public BasicTrackingState(IA_controller controller) : base(controller)
    {
        targetIsValid = true;
        targetInRange = false;
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
        controller.Move(controller.pathfinding.getMouvementVector(), controller.tracking_speed);
    }

    public override void animationTriggerIsCalled(IA_controller controller)
    {
    }

    public override void Enter()
    {
        base.Enter();
        targetInVision = controller.checkTargetInVision();
        (controller.data as Character.CharacterData).speed = controller.tracking_speed;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        targetIsValid = this.controller.target != null;
        targetInRange = controller.targetInRange(controller.attack_range);
        targetInVision = controller.checkTargetInVision();

        if (!targetIsValid) controller.changeState(controller.wandering_state);
        if (targetInRange && targetInVision) controller.changeState(controller.attack_state);
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
    }
}

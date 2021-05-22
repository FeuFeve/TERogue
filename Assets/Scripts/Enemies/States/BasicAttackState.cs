using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BasicAttackState : IActionState
{
    bool targetInAttackRange;
    bool targetIsValid;
    bool targetInVision;

    

    public BasicAttackState(IA_controller controller) : base(controller)
    {
        //ICI Ajouter les attaques désirées à la liste d'actions
        //Action range_attack = new DemonBoss_BasicRangeAttack(this);
        //actions.Add(range_attack);
        //...
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
    }

    public override void Enter()
    {
        base.Enter();
        targetInAttackRange = controller.targetInRange(controller.attack_range);
        targetIsValid = controller.target != null;
        targetInVision = controller.checkTargetInVision();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        targetIsValid = controller.target != null;
        targetInAttackRange = controller.targetInRange(controller.attack_range);
        targetInVision = controller.checkTargetInVision();

        if (currentAction == null)
        {
            if (!targetIsValid)
            {
                controller.changeState(controller.wandering_state);
                return;
            }

            if (!targetInVision || !targetInAttackRange)
            {
                controller.changeState(controller.tracking_state);
                return;
            }
        } else
        {
            if(currentAction.isRunning)
            {
                return;
            } else
            {
                if (!targetIsValid)
                {
                    controller.changeState(controller.wandering_state);
                    return;
                }

                if (!targetInVision || !targetInAttackRange)
                {
                    controller.changeState(controller.tracking_state);
                    return;
                }
            }
        }

        /**
        if (currentAction == null && targetIsValid && (!targetInAttackRange || !targetInVision)) controller.changeState(controller.tracking_state); return;
        if (currentAction == null && !targetIsValid) controller.changeState(controller.wandering_state); return; 
        if (!targetIsValid && !currentAction.isRunning && currentAction != null) controller.changeState(controller.wandering_state); return;
        if (targetIsValid && (!targetInVision || !targetInAttackRange) && (currentAction == null || !currentAction.isRunning)) controller.changeState(controller.tracking_state); return;
        **/
        //Debug.Log("range : " + targetInAttackRange + "; valid : " + targetIsValid + "; vision " + targetInVision + "; action " + currentAction.isRunning + " colliding "+ controller.isCollinding + "; result : " + (targetIsValid && (!targetInVision || !targetInAttackRange) && !currentAction.isRunning));
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
    }

    public override void animationTriggerIsCalled(IA_controller controller)
    {
        base.animationTriggerIsCalled(controller);
        if(currentAction != null) currentAction.animationTriggerIsCalled();
    }

    public void AddAction(Action action)
    {
        this.actions.Add(action);
    }
}

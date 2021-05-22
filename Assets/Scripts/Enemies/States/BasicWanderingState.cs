using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWanderingState : State
{
    Vector2 wandering_direction;
    private float wanderingStartTime;
    private float timeToWander;

    bool targetExists;
    bool targetInAttackRange;
    bool targetInVision;

    public BasicWanderingState(IA_controller controller) : base(controller)
    {
        changeWanderingDirection();
        targetExists = false;
    }

    public override void Act(IA_controller controller)
    {
        base.Act(controller);
        if (wandering_direction == null) changeWanderingDirection();
        Vector2 dir = controller.pathfinding.getMouvementByVector((wandering_direction + (Vector2)controller.collision_collider.transform.position), controller.collision_collider.transform.position);
        controller.Move(dir, controller.wandering_speed);
        if (wanderingStartTime + timeToWander <= Time.time)
        {
            changeWanderingDirection();
        }
    }

    public override void animationTriggerIsCalled(IA_controller controller)
    {
    }

    public override void Enter()
    {
        base.Enter();
        targetInVision = controller.checkTargetInVision();
        (controller.data as Character.CharacterData).speed = controller.wandering_speed; 
        changeWanderingDirection();
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        targetExists = (controller.target != null);
        targetInAttackRange = controller.targetInRange(controller.attack_range);

        if ( targetExists && (!targetInAttackRange || !targetInVision)) controller.changeState(controller.tracking_state);
        if (targetInVision && targetInAttackRange) controller.changeState(controller.attack_state); 
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
    }

    private void changeWanderingDirection()
    {
        this.wandering_direction = getRandomVect2();
        this.timeToWander = Random.Range(2f, 5f);
        this.wanderingStartTime = Time.time;
    }

    Vector2 getRandomVect2()
    {
        return Random.insideUnitCircle.normalized; ;
    }
}

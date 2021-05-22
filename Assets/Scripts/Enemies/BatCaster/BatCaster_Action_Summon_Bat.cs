using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCaster_Action_Summon_Bat : Action
{
    private bool casting;

    public BatCaster_Action_Summon_Bat(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        if (!casting) End();
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        BatCaster_IAController controller = (BatCaster_IAController)caller.controller;
        Vector2 randVec = Random.insideUnitCircle;
        randVec.Normalize();
        Vector2 spawnPos = caller.controller.pathfinding.getValidSpawnPos((Vector2)this.caller.controller.collision_collider.transform.position + this.caller.controller.collision_collider.offset + randVec);
        GameObject go = Object.Instantiate(controller.toSummon, spawnPos, Quaternion.identity);
        FlyingPathfinding toRegister = go.GetComponent<FlyingPathfinding>();
        if (toRegister != null) toRegister.registerManager(controller.pathManager);
        casting = false;
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        casting = true;
        caller.controller.animator.SetTrigger("Attacking");
    }
}

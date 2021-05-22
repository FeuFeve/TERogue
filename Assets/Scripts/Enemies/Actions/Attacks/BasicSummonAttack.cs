using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSummonAttack : Action
{
    private bool casting;
    GameObject toSummon;
    bool doesFly;
    public BasicSummonAttack(IActionState caller, float cooltime, GameObject toSummon, bool doesFly = false) : base(caller, cooltime)
    {
        this.toSummon = toSummon;
        this.doesFly = doesFly;
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        if (!casting) End();
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        IA_controller controller = caller.controller;
        Vector2 randVec = Random.insideUnitCircle;
        randVec.Normalize();
        Vector2 spawnPos = caller.controller.pathfinding.getValidSpawnPos((Vector2)this.caller.controller.collision_collider.transform.position + this.caller.controller.collision_collider.offset + randVec);
        GameObject go = Object.Instantiate(toSummon, spawnPos, Quaternion.identity);
        IA_controller summoned_controller = go.GetComponent<IA_controller>();
        summoned_controller.goldAmount = 0;
        if (doesFly)
        {
            FlyingPathfinding toRegister = go.GetComponent<FlyingPathfinding>();
            if (toRegister != null) toRegister.registerManager(controller.pathfinding.GetPathManager());
        } else
        {
            Pathfinding toRegister = go.GetComponent<Pathfinding>();
            if (toRegister != null) toRegister.registerManager(controller.pathfinding.GetPathManager());
        }
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

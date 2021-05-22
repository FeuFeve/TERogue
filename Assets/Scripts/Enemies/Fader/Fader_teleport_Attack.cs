using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader_teleport_Attack : Action
{
    private bool fading;
    public float max_distance = 12f;
    private float max_duration = 4f;
    private float startTime;
    public Fader_teleport_Attack(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        
        if(isRunning && max_duration + startTime < Time.time)
        {
            caller.controller.collision_collider.enabled = true;
            
            End();
        }
        
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        startTime = Time.time;
        if (fading)
        {
            Vector2 offset = Random.insideUnitCircle;
            Vector2 startPos = caller.controller.target == null ? caller.controller.collision_collider.transform.position : caller.controller.target.position;
            Vector2 pos = caller.controller.pathfinding.getValidSpawnPos(startPos + offset.normalized * Random.Range(0, max_distance));
            caller.controller.animator.SetTrigger("Fade_in");
            caller.controller.gameObject.transform.position = pos;
            fading = false;
        }
        else
        {
            caller.controller.Shoot(caller.controller.targetDir());
            caller.controller.collision_collider.enabled = true;
            End();
        }
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        startTime = Time.time;
        this.fading = true;
        caller.controller.animator.SetTrigger("Fade_out");
        caller.controller.collision_collider.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss_rush : Action
{

    private float max_rush_time = 3f;
    private float start_time;
    private Vector2 rush_dir;
    private float rush_speed = 11f;
    private bool casting;

    private bool rushing;

    private ScreenShaker shaker;
    public DemonBoss_rush(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);

        if (casting)
        {

        }
        else if (rushing)
        {
            caller.controller.Move(rush_dir, rush_speed);
            if (caller.controller.isCollinding && start_time + 0.2f <= Time.time)
            {
                rushing = false;
                shaker.TriggerShake(0.5f);
            }
        }
        else if (!casting && !rushing) End();

        if (rushing && start_time + max_rush_time <= Time.time)
        {
            rushing = false;
        }
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        this.rush_dir = caller.controller.targetDir();
        this.start_time = Time.time;
        rushing = true;
        casting = false;
        caller.controller.animator.SetBool("Rushing", rushing);
        
    }

    public override void End()
    {
        base.End();
        this.casting = false;
        this.rushing = false;
        caller.controller.animator.SetBool("Rushing", rushing);
    }

    public override void Start()
    {
        base.Start();
        this.casting = true;
        this.rushing = false;
        this.shaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShaker>();
        caller.controller.Move(Vector2.zero);
        caller.controller.animator.SetTrigger("Attacking");
    }

    public void setRushSpeed(float speed)
    {
        this.rush_speed = speed;
    }
}

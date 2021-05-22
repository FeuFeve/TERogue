using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undying_Boar_Rush_Attack : Action
{
    private float max_rush_time = 3f;
    private float start_time;
    private Vector2 rush_dir;
    private float rush_speed = 10f;
    private bool casting;
    private float shakeDuration;
    private bool shakeScreen;

    private bool rushing;

    private ScreenShaker shaker;
    public Undying_Boar_Rush_Attack(IActionState caller, float cooltime, bool shakeScreen, float shakeDuration = 0) : base(caller, cooltime)
    {
        this.shakeDuration = shakeDuration;
        this.shakeScreen = shakeScreen;
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
                if (shakeScreen) shaker.TriggerShake(shakeDuration);
                int shots = 6;
                for (int i = 0; i < shots; i++)
                {

                    var rotation = caller.controller.gameObject.transform.rotation;
                    var rotation_mod = Quaternion.AngleAxis((i / (float)shots) * 360, caller.controller.gameObject.transform.forward);
                    var direction = rotation * rotation_mod * Vector2.right;

                    caller.controller.Shoot(direction);
                }
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
    }

    public override void End()
    {
        base.End();
        this.casting = false;
        this.rushing = false;
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

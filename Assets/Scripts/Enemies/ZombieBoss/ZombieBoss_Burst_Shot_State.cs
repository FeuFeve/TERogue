using UnityEngine;

internal class ZombieBoss_Burst_Shot_State : Action
{
    private Weapon weapon;
    private float duration = 3f;
    private float startTime;
    private bool shooting;
    private bool casting;
    public ZombieBoss_Burst_Shot_State(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        if (shooting && startTime + duration > Time.time && !casting)
        {
            weapon.ShootDirection(caller.controller.targetDir());
        }
        else if (!casting && !shooting && startTime + duration < Time.time)
        {
            weapon.StopShooting();
            End();
        }
        else if(shooting && startTime + duration < Time.time)
        {
            shooting = false;
        }
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        casting = false;
        shooting = true;
        startTime = Time.time;
        weapon.StartShooting();
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        shooting = false;
        casting = true;
        this.weapon = caller.controller.weapon;
        caller.controller.animator.SetTrigger("Attacking");
    }
}
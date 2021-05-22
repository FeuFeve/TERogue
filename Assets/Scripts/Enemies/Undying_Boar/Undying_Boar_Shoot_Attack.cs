using UnityEngine;

internal class Undying_Boar_Shoot_Attack : Action
{
    bool attacking;

    public Undying_Boar_Shoot_Attack(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State state)
    {
        base.Act(state);

        if (!attacking)
        {
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
        this.attacking = true;
        this.caller.controller.Move(Vector2.zero);
        this.caller.controller.animator.SetTrigger("Attacking");
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        int shots = 4;
        for (int i = 0; i < shots; i++)
        {

            var rotation = caller.controller.gameObject.transform.rotation;
            var rotation_mod = Quaternion.AngleAxis((i / (float)shots) * 360, caller.controller.gameObject.transform.forward);
            var direction = rotation * rotation_mod * Vector2.right;

            caller.controller.Shoot(direction);
        }
        this.attacking = false;
    }
}
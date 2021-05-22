using UnityEngine;

internal class ZombieBoss_Throw_Pickmin_Attack : Action
{
    private ZombieBoss_Attack_State zombieBoss_Attack_State;
    private float v;
    private GameObject toThrow;
    private float projSpeed = 6f;

    public ZombieBoss_Throw_Pickmin_Attack(IActionState caller, float cooltime, GameObject toThrow) : base(caller, cooltime)
    {
        this.toThrow = toThrow;
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        GameObject go = GameObject.Instantiate(toThrow, caller.controller.gameObject.transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().velocity = caller.controller.targetDir().normalized * projSpeed;
        End();
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        caller.controller.animator.SetTrigger("Attacking");
    }
}
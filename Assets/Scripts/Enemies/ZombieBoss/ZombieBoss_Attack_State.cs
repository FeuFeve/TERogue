using UnityEngine;

internal class ZombieBoss_Attack_State : BasicAttackState
{
    GameObject toThrow;
    public ZombieBoss_Attack_State(IA_controller controller, GameObject toThrow) : base(controller)
    {
        this.actions.Add(new ZombieBoss_Jump_Attack(this, 8f));
        this.actions.Add(new ZombieBoss_Throw_Pickmin_Attack(this, 8f, toThrow));
        this.actions.Add(new ZombieBoss_Burst_Shot_State(this, 8f));
    }

}
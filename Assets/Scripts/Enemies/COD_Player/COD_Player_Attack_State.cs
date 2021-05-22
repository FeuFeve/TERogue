using UnityEngine;

internal class COD_Player_Attack_State : BasicAttackState
{
    public COD_Player_Attack_State(IA_controller controller, LineRenderer indicator) : base(controller)
    {
        this.actions.Add(new COD_Player_Snipe_Attack(this, 5f, indicator));
    }
}
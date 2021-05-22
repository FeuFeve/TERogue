internal class Undying_Boar_Attack_State_P1 : BasicAttackState
{
    public Undying_Boar_Attack_State_P1(IA_controller controller) : base(controller)
    {
        this.actions.Add(new Undying_Boar_Rush_Attack(this, 2.5f, true, 0.5f));
    }
}
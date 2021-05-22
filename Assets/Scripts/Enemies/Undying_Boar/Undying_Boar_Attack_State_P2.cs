internal class Undying_Boar_Attack_State_P2 : BasicAttackState
{
    public Undying_Boar_Attack_State_P2(IA_controller controller) : base(controller)
    {
        Undying_Boar_Rush_Attack rush = new Undying_Boar_Rush_Attack(this, 1f, true, 0.5f);
        rush.setRushSpeed(14f);
        this.actions.Add(rush);
        this.actions.Add(new Undying_Boar_Shoot_Attack(this, 1f));
    }
}
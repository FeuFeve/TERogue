using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Attack_State : BasicAttackState
{
    public Mimic_Attack_State(IA_controller controller) : base(controller)
    {
        this.actions.Add(new Mimic_eat_attack(this, 0f));
        this.actions.Add(new BasicRangeAttack(this, 2f));
    }

}

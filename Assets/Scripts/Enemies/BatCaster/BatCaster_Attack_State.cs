using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCaster_Attack_State : BasicAttackState
{
    GameObject toSummon;
    public BatCaster_Attack_State(IA_controller controller, GameObject toSummon) : base(controller)
    {
        this.toSummon = toSummon;
        this.actions.Add(new BasicSummonAttack(this, 5f, this.toSummon, true));
        this.actions.Add(new BasicRangeAttack(this, 1.5f));
    }


}

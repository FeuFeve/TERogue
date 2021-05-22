using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeed : StatusEffect {
    protected override void OnBegin () {
        base.OnBegin();
        AddStackOnApply = true;
        duration = 2;
        ResetDurationOnApply = true;
    }

    public override void OnApply () {
        base.OnApply();
        if (Stacks > 10) {
            stacks = 10;
            return;
        }
        GetComponent<Weapon>().delayMult = GetComponent<Weapon>().delayMult * 0.9f;
        
    }

    protected override void OnExpire () {
        GetComponent<Weapon>().delayMult = 1;
        base.OnExpire();
    }
}

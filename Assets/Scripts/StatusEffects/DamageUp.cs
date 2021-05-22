using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUp : StatusEffect {
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
        GetComponent<Weapon>().damageMult = GetComponent<Weapon>().damageMult * 1.1f;
    }

    protected override void OnExpire () {
        base.OnExpire();
        GetComponent<Weapon>().damageMult = 1;
    }
}

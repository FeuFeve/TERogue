using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : StatusEffect {
    protected float damage = 10;

    protected override void OnBegin () {
        duration = 0.1f;
        tickEvery = 0.1f;
        AddStackOnApply = false;
        damage = caster.GetComponent<Weapon>().projectileData.damage / 10;
        base.OnBegin();
    }

    public override void OnApply () {
        GetComponent<Character>().Heal(damage * Stacks, HpChangesType.lifeLeechHeal);
        base.OnApply();
        base.OnExpire();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : StatusEffect {
    protected float damage = 10;

    protected override void OnBegin () {
        AddStackOnApply = true;
        damage = caster.GetComponent<Weapon>().projectileData.damage / 10;
        base.OnBegin();
    }

    protected override void OnTick () {
        GetComponent<Character>().Damage(damage * Stacks, HpChangesType.bleedDamages);
    }
}

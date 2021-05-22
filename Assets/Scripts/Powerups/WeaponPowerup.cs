using UnityEngine;

public class WeaponPowerup : Powerup {

    public Weapon.WeaponData add;
    public Weapon.WeaponData mul;
    public Weapon.WeaponData set;

    void OnTriggerEnter2D (Collider2D collider) {
        Weapon w = collider.gameObject.GetComponent<Weapon>();
        if (w == null) return;

        base.OnTriggerEnter2D(collider);

        w.data.Add(add);
        w.data.Mul(mul);
        w.data.Set(set);
        PrintInfo();
    }
}

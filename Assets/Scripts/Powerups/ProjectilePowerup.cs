using UnityEngine;

public class ProjectilePowerup : Powerup {

    public Projectile.ProjectileData add;
    public Projectile.ProjectileData mul;
    public Projectile.ProjectileData set;
    public GameObject newProjectilePrefab;

    void OnTriggerEnter2D (Collider2D collider) {
        Weapon w = collider.gameObject.GetComponent<Weapon>();
        if (w == null) return;

        base.OnTriggerEnter2D(collider);

        if (newProjectilePrefab != null)
            w.projectilePrefab = newProjectilePrefab;

        Projectile.ProjectileData pd = collider.gameObject.GetComponent<Weapon>().projectileData;
        pd.Add(add);
        pd.Mul(mul);
        pd.Set(set);
        PrintInfo();
    }
}

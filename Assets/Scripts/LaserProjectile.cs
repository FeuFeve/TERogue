using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : Projectile {
    public LineRenderer lineRenderer;
    public float alive = 0.5f;
    private float startAlive;
    int mask = 0;

    void Start () {
        startAlive = alive;

        if (caster.GetComponent<Character>() is Player) {
            mask = 1 << 10;
            mask = mask | (1 << 17);
            mask = mask | (1 << 13);
        } else {
            mask = 1 << 11;
            mask = mask | (1 << 13);
        }

        RaycastHit2D hit;
        Vector3 nextPos;
        Vector2 origin;
        Vector2 destination;
        lineRenderer.SetPosition(0, transform.position);
        for (int i = 1; i < 40; i++) {
            Homing();
            nextPos = transform.position + transform.right * (data as ProjectileData).speed / 10f;
            origin.x = transform.position.x;
            origin.y = transform.position.y;
            destination.x = nextPos.x;
            destination.y = nextPos.y;
            hit = Physics2D.Raycast(origin, (destination - origin).normalized, (origin - destination).magnitude, mask);
            if (hit.collider != null) {
                Character c = hit.collider.gameObject.GetComponent<Character>();
                if (c != null) {
                    ExplosiveDamage();
                    HitDamage(hit.collider);
                }

                nextPos.x = hit.point.x;
                nextPos.y = hit.point.y;
                lineRenderer.SetPosition(i, nextPos);
                lineRenderer.positionCount = i;
                break;
            } else {
                lineRenderer.SetPosition(i, nextPos);
            }

            transform.position = nextPos;
        }
    }

    void Update () {
        alive -= Time.deltaTime;

        lineRenderer.widthMultiplier = alive / startAlive;

        if (alive <= 0) {
            Destroy(gameObject);
        }
    }

    protected override void Homing () {
        if ((data as ProjectileData).homingSpeed <= 0) return;

        if (homingTarget == null) {
            GetHomingTarget();
            return;
        }

        float y = transform.InverseTransformPoint(homingTarget.transform.position).y;
        if (y > 0) {
            transform.Rotate(new Vector3(0, 0, (data as ProjectileData).homingSpeed * ((data as ProjectileData).speed / 10f) * 10));
        } else {
            transform.Rotate(new Vector3(0, 0, -(data as ProjectileData).homingSpeed * ((data as ProjectileData).speed / 10f) * 10));
        }
    }

    protected override void HitDamage (Collider2D collider) {
        Character c = collider.gameObject.GetComponent<Character>();
        if (c != null) {
            if (Random.Range(0f, 1f) < (data as ProjectileData).critChance) {
                c.Damage((data as ProjectileData).damage * (data as ProjectileData).critDamage, HpChangesType.criticalDamages);

                foreach (KeyValuePair<string, OnCrit> onCritPair in caster.onCrits) {
                    OnCrit onCrit = onCritPair.Value;
                    if (onCrit.Roll()) {
                        c.AddStatusEffect(onCrit.ApplyOnTarget, caster.gameObject);
                        caster.AddStatusEffect(onCrit.ApplyOnCaster, caster.gameObject);
                    }
                }
            } else {
                c.Damage((data as ProjectileData).damage, HpChangesType.normalDamages);
            }

            foreach (KeyValuePair<string, OnHit> onHitPair in caster.onHits) {
                OnHit onHit = onHitPair.Value;
                if (onHit.Roll()) {
                    c.AddStatusEffect(onHit.ApplyOnTarget, caster.gameObject);
                    caster.AddStatusEffect(onHit.ApplyOnCaster, caster.gameObject);
                }
            }
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {

    }
}

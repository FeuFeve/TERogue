using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Powerupable {
    private float aliveTimer = -1;
    private float currentSpeed = -1;
    private bool bounce = false;
    private Rigidbody2D rb;
    public Character caster;
    protected GameObject homingTarget;
    public GameObject explosion;

    public Transform skin;
    public Transform shadow;

    [System.Serializable]
    public class ProjectileData : Powerupable.PowerupableData {
        public float alive;
        public float speed;
        public float acceleration;
        public float damage;
        public int children;
        public int depth; //Stop making children after n generation
        public float homingSpeed; //Changes the angle of the projectile at this rate to face the closest enemy
        public float explosionRadius;
        public float explosionDamage;
        public bool bounce;
        public float critChance;
        public float critDamage;

        public override void Add(PowerupableData d) {
            if (!(d is ProjectileData)) return;
            ProjectileData pd = (ProjectileData)d;
            
            alive += pd.alive;
            speed += pd.speed;
            acceleration += pd.acceleration;
            damage += pd.damage;
            children += pd.children;
            depth += pd.depth;
            homingSpeed += pd.homingSpeed;
            explosionRadius += pd.explosionRadius;
            explosionDamage += pd.explosionDamage;
            critChance += pd.critChance;
            critDamage += pd.critDamage;
            bounce = bounce || pd.bounce;

            base.Add(d);
        }

        public override void Mul(PowerupableData d) {
            if (!(d is ProjectileData)) return;
            ProjectileData pd = (ProjectileData)d;
            
            alive *= pd.alive;
            speed *= pd.speed;
            acceleration *= pd.acceleration;
            damage *= pd.damage;
            children *= pd.children;
            depth *= pd.depth;
            homingSpeed *= pd.homingSpeed;
            explosionRadius *= pd.explosionRadius;
            explosionDamage *= pd.explosionDamage;
            critChance *= pd.critChance;
            critDamage *= pd.critDamage;
            bounce = bounce && pd.bounce;

            base.Mul(d);
        }

        public override void Set(PowerupableData d) {
            if (!(d is ProjectileData)) return;
            ProjectileData pd = (ProjectileData)d;
            
            if (pd.alive != -999) alive = pd.alive;
            if (pd.speed != -999) speed = pd.speed;
            if (pd.acceleration != -999) acceleration = pd.acceleration;
            if (pd.damage != -999) damage = pd.damage;
            if (pd.children != -999) children = pd.children;
            if (pd.depth != -999) depth = pd.depth;
            if (pd.homingSpeed != -999) homingSpeed = pd.homingSpeed;
            if (pd.explosionRadius != -999) explosionRadius = pd.explosionRadius;
            if (pd.explosionDamage != -999) explosionDamage = pd.explosionDamage;
            if (pd.critChance != -999) critChance = pd.critChance;
            if (pd.critDamage != -999) critDamage = pd.critDamage;

            base.Set(d);
        }

        public override void Check() {
            if (alive < 1) alive = 1;
            if (speed < 0) speed = 0;
            if (damage < 0.1) damage = 0.1f;
            if (children < 1) children = 1;
            if (depth < 1) depth = 1;
            if (explosionRadius < 0) explosionRadius = 0;
            if (explosionDamage < 0) explosionDamage = 0;
            if (explosionDamage > 1) explosionDamage = 1;
            if (critChance > 1) critChance = 1;
            if (critDamage < 1) critDamage = 1;
        }

        public override PowerupableData Clone() {
            ProjectileData pd = new ProjectileData();

            pd.alive = alive;
            pd.speed = speed;
            pd.acceleration = acceleration;
            pd.damage = damage;
            pd.children = children;
            pd.depth = depth;
            pd.homingSpeed = homingSpeed;
            pd.explosionRadius = explosionRadius;
            pd.explosionDamage = explosionDamage;
            pd.bounce = bounce;
            pd.critChance = critChance;
            pd.critDamage = critDamage;

            return pd;
        }

    }

    public override void SetData(PowerupableData d) {
        base.SetData((d as ProjectileData));

        aliveTimer = (data as ProjectileData).alive;
        currentSpeed = (data as ProjectileData).speed;
        bounce = (data as ProjectileData).bounce;
    }
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (caster is Player) {
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectiles");
        } else {
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectiles");
        }
    }

    // Update is called once per frame
    void Update() {
        AliveTimer();
        Homing();
        Move();
        Animate();
    }

    public void Animate() {
        shadow.localEulerAngles = -transform.localEulerAngles;
        skin.localEulerAngles = transform.localEulerAngles;
    }

    protected void GetHomingTarget () {
        float minDist = 999;
        if (caster is Player) {
            foreach(GameObject enemy in GameData.enemies) {
                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist < minDist) {
                    minDist = dist;
                    homingTarget = enemy;
                }
            }
        } else {
            foreach (GameObject enemy in Player.playerList) {
                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist < minDist) {
                    minDist = dist;
                    homingTarget = enemy;
                }
            }
        }
    }

    protected virtual void Homing() {
        if ((data as ProjectileData).homingSpeed <= 0) return;

        if (homingTarget == null) {
            GetHomingTarget();
            return;
        }

        float y = transform.InverseTransformPoint(homingTarget.transform.position).y;
        if (y > 0) {
            transform.Rotate(new Vector3(0, 0, (data as ProjectileData).homingSpeed * ((data as ProjectileData).speed / 10f)));
        } else {
            transform.Rotate(new Vector3(0, 0, -(data as ProjectileData).homingSpeed * ((data as ProjectileData).speed / 10f)));
        }

    }

    private void AliveTimer() {
        //Killing the projectile after n seconds
        aliveTimer -= Time.deltaTime;
        if (aliveTimer <= 0) {
            SpawnChildren();
            Die();
        }
    }

    private void Move() {
        //Moving the projectile
        currentSpeed += (data as ProjectileData).acceleration * Time.deltaTime;
        if (currentSpeed < 0)
            currentSpeed = 0;
        rb.velocity =  transform.right * currentSpeed;
    }

    private void Die() {
        if (explosion != null) {
            GameObject e = Instantiate(explosion, transform.parent);
            e.transform.position = transform.position;
        }
        Destroy(gameObject);
    }

    private void SpawnChildren () {
        //We decrease the depth so we dont spawn children indefinitely
        (data as ProjectileData).depth--;

        //Spawning n children
        if ((data as ProjectileData).depth > 0) {
            for (int i = 0; i < (data as ProjectileData).children; i++) {
                GameObject bullet = Instantiate(transform.gameObject, transform.position, transform.rotation, transform.parent);
                bullet.GetComponent<Projectile>().SetData(data);
                bullet.GetComponent<Projectile>().enabled = true;
                bullet.transform.Rotate(Vector3.forward, Random.Range(10, -10));
            }
        }
    }

    protected void ExplosiveDamage () {
        if ((data as ProjectileData).explosionRadius > 0) {
            foreach (GameObject enemy in GameData.enemies) {
                if ((enemy.transform.position - transform.position).magnitude < (data as ProjectileData).explosionRadius) {
                    enemy.GetComponent<Character>().Damage((data as ProjectileData).explosionDamage * (data as ProjectileData).damage, HpChangesType.normalDamages);
                }
            }
        }
    }

    protected virtual void HitDamage (Collider2D collider) {
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
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ExplosiveDamage();
        HitDamage(collision.collider);

        //Debug.Log(collision.contacts.Length);

        Vector2 diff = Vector3.Reflect(transform.right, collision.contacts[0].normal);
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        transform.Translate(Vector3.right * 0.1f, Space.Self);
        if (!bounce) {
            SpawnChildren();
            Die();
        } else {
            bounce = false;
        }
    }
}

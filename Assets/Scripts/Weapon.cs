using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Powerupable {
    public float coroutineWakeupAt = 0;
    public float delayMult = 1;
    public float damageMult = 1;
    public static Transform projectilesParentTransform;
    [System.Serializable]
    public class WeaponData : Powerupable.PowerupableData {
        public float maxHeat;
        public float heatPerBullet;
        public float heatLossPerSecond;
        public float overheatCooldownTime;
        public float bullets;
        public float burst;
        public float spread;
        public float burstDelay;
        public float shotDelay;
        public bool cantOverheat;
        public bool constantSpread;
        public bool semiAutomatic;

        public override void Add(Powerupable.PowerupableData d) {
            if (!(d is WeaponData)) return;
            WeaponData wd = (WeaponData)d;

            maxHeat += wd.maxHeat;
            heatPerBullet += wd.heatPerBullet;
            heatLossPerSecond += wd.heatLossPerSecond;
            overheatCooldownTime += wd.overheatCooldownTime;
            bullets += wd.bullets;
            burst += wd.burst;
            spread += wd.spread;
            burstDelay += wd.burstDelay;
            shotDelay += wd.shotDelay;

            cantOverheat = cantOverheat || wd.cantOverheat;
            constantSpread = constantSpread || wd.constantSpread;
            semiAutomatic = semiAutomatic || wd.semiAutomatic;
            Check();
        }

        public override void Mul(Powerupable.PowerupableData d) {
            if (!(d is WeaponData)) return;
            WeaponData wd = (WeaponData)d;
            
            maxHeat *= wd.maxHeat;
            heatPerBullet *= wd.heatPerBullet;
            heatLossPerSecond *= wd.heatLossPerSecond;
            overheatCooldownTime *= wd.overheatCooldownTime;
            bullets *= wd.bullets;
            burst *= wd.burst;
            spread *= wd.spread;
            burstDelay *= wd.burstDelay;
            shotDelay *= wd.shotDelay;

            cantOverheat = cantOverheat && wd.cantOverheat;
            constantSpread = constantSpread && wd.constantSpread;
            semiAutomatic = semiAutomatic && wd.semiAutomatic;
            Check();
        }

        public override void Set (Powerupable.PowerupableData d) {
            if (!(d is WeaponData)) return;
            WeaponData wd = (WeaponData)d;

            if (wd.maxHeat != -999) maxHeat = wd.maxHeat;
            if (wd.heatPerBullet != -999) heatPerBullet = wd.heatPerBullet;
            if (wd.heatLossPerSecond != -999) heatLossPerSecond = wd.heatLossPerSecond;
            if (wd.overheatCooldownTime != -999) overheatCooldownTime = wd.overheatCooldownTime;
            if (wd.bullets != -999) bullets = wd.bullets;
            if (wd.burst != -999) burst = wd.burst;
            if (wd.spread != -999) spread = wd.spread;
            if (wd.burstDelay != -999) burstDelay = wd.burstDelay;
            if (wd.shotDelay != -999) shotDelay = wd.shotDelay;
            Check();
        }

        public override void Check() {
            if (maxHeat < 25) maxHeat = 25;
            if (heatPerBullet < 1) heatPerBullet = 1;
            if (heatLossPerSecond < 1) heatLossPerSecond = 1;
            if (overheatCooldownTime < 1) overheatCooldownTime = 1;
            if (bullets < 1) bullets = 1;
            if (burst < 1) burst = 1;
            if (spread < 0) spread = 0;
            if (burstDelay < 0) burstDelay = 0;
            if (burstDelay > 0.05f) burstDelay = 0.05f;
            if (shotDelay < 0.01f) shotDelay = 0.01f;
            if (shotDelay > 0.75f) shotDelay = 0.75f;
        }

        public override PowerupableData Clone() {
            WeaponData wd = new WeaponData();

            wd.maxHeat = maxHeat;
            wd.heatPerBullet = heatPerBullet;
            wd.heatLossPerSecond = heatLossPerSecond;
            wd.overheatCooldownTime = overheatCooldownTime;
            wd.bullets = bullets;
            wd.burst = burst;
            wd.spread = spread;
            wd.burstDelay = burstDelay;
            wd.shotDelay = shotDelay;
            wd.cantOverheat = cantOverheat;
            wd.constantSpread = constantSpread;
            wd.semiAutomatic = semiAutomatic;

            return wd;           
        }
    }

    private int burstLeft = 0;

    public GameObject projectilePrefab;

    private float heat;
    private float overheatedCooldown;
    private float angle = 0;



    private bool overheated = false;
    public bool shootInput = false;

    public Projectile.ProjectileData projectileData = new Projectile.ProjectileData();
    [SerializeField] private WeaponData wd;

    // UI
    public Bar heatBar; // Use only if the weapon is linked to a player

    // Start is called before the first frame update
    void Start() {
        coroutineWakeupAt = Time.time;

        if (projectilesParentTransform == null) {
            Transform current = transform;
            while (current.parent != null) {
                current = current.parent;
            }
            projectilesParentTransform = current.parent;
        }
        SetData(wd);

        // UI
        if (heatBar)
        {
            heatBar.SetMax(wd.maxHeat);
            heatBar.SetValue(0);
        }
        

        StartCoroutine("shootingCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        WeaponData wData = data as WeaponData;

        if (overheated) {
            overheatedCooldown -= Time.deltaTime;
            if (overheatedCooldown < 0) {
                heat -= wData.heatLossPerSecond * Time.deltaTime;
                if (heat < wData.maxHeat - (wData.heatPerBullet * wData.bullets * 2) || heat < 0) { // Temps d'attente suplémentaire, on attends que la barre déscende un peu !
                    overheated = false;
                    shootInput = false;
                }
            }
        } else {
            heat -= wData.heatLossPerSecond * Time.deltaTime;
            if (heat < 0)
                heat = 0;
        }

        // UI (update last)
        if (heatBar)
        {
            heatBar.SetMax(wData.maxHeat);
            heatBar.SetValue(heat);

            if (overheated)
                heatBar.Blink();
            else
                heatBar.ResetBlink();
        }
    }

    IEnumerator shootingCoroutine () {
        yield return new WaitForSeconds(coroutineWakeupAt - Time.time);
        float delay = 0;

        burstLeft = (int)(data as WeaponData).burst;
        while (true) {
            if (shootInput) { //shot
                while (burstLeft > 0 && !overheated) { //burst
                    burstLeft--;
                    SpawnBullet();

                    delay = (data as WeaponData).burstDelay * delayMult + 0.001f;
                    coroutineWakeupAt = Time.time + delay;
                    yield return new WaitForSeconds(delay);
                }
                if ((data as WeaponData).semiAutomatic) shootInput = false;
                burstLeft = (int)(data as WeaponData).burst;

                delay = (data as WeaponData).shotDelay * delayMult + 0.001f;
                coroutineWakeupAt = Time.time + delay;
                yield return new WaitForSeconds(delay);
            }
            yield return false;
        }
    }


    public void Shoot (Vector3 d) {
        ShootDirection(d);
        StartShooting();
    }

    public void ShootDirection(Vector3 d) {
        angle = Vector3.Angle(Vector3.right, d);
        if (d.y < 0) angle = -angle;
    }

    public void StartShooting () {
        shootInput = true;
        //Debug.Log(gameObject.name + " Burst: " + (data as WeaponData).burstDelay * delayMult + 0.001f + " B: " + (data as WeaponData).burstDelay + " Mult: " + delayMult);

    }

    public void StopShooting () {
        shootInput = false;
    }

    void SpawnBullet() {
        WeaponData wData = data as WeaponData;
        for (int i = 0; i < wData.bullets; i++) {
            GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation, projectilesParentTransform);
            bullet.GetComponent<Projectile>().SetData(projectileData);
            bullet.GetComponent<Projectile>().caster = GetComponent<Character>();
            (bullet.GetComponent<Projectile>().data as Projectile.ProjectileData).damage = (bullet.GetComponent<Projectile>().data as Projectile.ProjectileData).damage * damageMult;

            float rotation = 0;
            if (wData.constantSpread) {
                float baseRotation = 0;
                float shotRotation = 0;

                if (wData.burst > 1)
                    baseRotation = (((float)(burstLeft) / (wData.burst - 1)) * 2) - 1;
                if (wData.bullets > 1)
                    shotRotation = (((float)(i) / (wData.bullets - 1)) * 2) - 1;


                rotation = baseRotation + shotRotation;
                rotation *= wData.spread;

                //Debug.Log(baseRotation + " " + shotRotation + " " + rotation);
            } else {
                rotation = Random.Range(-wData.spread, wData.spread);
            }

            bullet.transform.Rotate(Vector3.forward, angle + rotation);

            heat += wData.heatPerBullet;
            if (heat >= wData.maxHeat)
            {
                if (!wData.cantOverheat)
                {
                    overheated = true;
                    shootInput = false;
                    overheatedCooldown = wData.overheatCooldownTime;
                }
                heat = wData.maxHeat;
                //break; Tirer ou non toute les balles
            }
        }
    }

    public void ForceShoot(Vector2 direction)
    {
        ShootDirection(direction);
        SpawnBullet();
    }
}


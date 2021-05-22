using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_controller : Character
{

    //Components
    public CircleCollider2D vision_collider;
    public Collider2D collision_collider;
    public Weapon weapon;
    public Animator animator;
    public Pathfinding pathfinding;

    //Stats
    public float tracking_speed;
    public float wandering_speed;
    public float vision_range;
    public float attack_range;
    public bool doContactDmg = false;
    public float contact_damage;


    //States
    public State attack_state;
    public State wandering_state;
    public State tracking_state;

    //Variables
    public List<Transform> targets;
    public Transform target;
    public State currentState;
    protected float cacTickTime = 0.8f;
    protected float lastTickTime;
    public bool isCollinding;

    //Loot
    public int scoreValue = 5;
    public int goldAmount = 0;
    public GameObject goldPrefab;


    // Start is called before the first frame update
    public override void Start()
    {

        base.Start();

        //Buff
        int world = GameData.level / 10;
        float statBuff = Mathf.Pow(((float)world / 10f) + 1f, 2); //((world/5) + 1)²    starts at 1, n²
        (data as CharacterData).maxHP = (data as CharacterData).maxHP * statBuff;
        HP = (data as CharacterData).maxHP;
        Weapon w = GetComponent<Weapon>();
        if (w != null) {
            w.projectileData.damage = w.projectileData.damage * statBuff;
        }

        this.attack_state = new BasicAttackState(this);
        this.wandering_state = new BasicWanderingState(this);
        this.tracking_state = new BasicTrackingState(this);
        this.lastTickTime = Time.time;
        vision_collider.radius = vision_range;
        this.target = null;
        this.targets = new List<Transform>();

        this.weapon = GetComponent<Weapon>();
        this.animator = GetComponentInChildren<Animator>();
        this.pathfinding = GetComponent<Pathfinding>();

        this.currentState = wandering_state;
        currentState.Enter();

        if (!GameData.enemies.Contains(this.gameObject))
            GameData.enemies.Add(this.gameObject);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        updateIsColliding();
        changeTarget();
        Animate();

        currentState.Act(this);
        currentState.PhysicUpdate();
        currentState.LogicUpdate();
    }

    private void changeTarget()
    {
        cleanTargets();
        if (this.targets.Count != 0)
        {
            this.target = targets[0];
            float minDist = Vector2.Distance(this.target.position, gameObject.transform.position);
            foreach (Transform trans in targets)
            {
                if (trans.gameObject.activeInHierarchy == false) continue;
                if (Vector2.Distance(trans.position, gameObject.transform.position) < minDist)
                {
                    minDist = Vector2.Distance(trans.position, gameObject.transform.position);
                    this.target = trans;
                }
            }
            if (this.target.gameObject.activeInHierarchy == false) this.target = null;
        }
        else
        {
            this.target = null;
        }
        pathfinding.setTarget(target);
    }

    public virtual void Animate()
    {
        float currentSpeed = rb.velocity.magnitude;
        animator.SetFloat("Speed", Mathf.Min(currentSpeed - 0.1f, 10f));
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        contactDamage(collision);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (targets == null) this.targets = new List<Transform>();
            foreach (GameObject player in Player.playerList)
            {
                if (player.activeInHierarchy && !targets.Contains(player.transform)) targets.Add(player.transform);
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        contactDamage(collision);
    }

    public virtual void animationTriggerIsCalled()
    {
        currentState.animationTriggerIsCalled(this);
    }

    public void changeState(State state)
    {
        this.currentState.Exit();
        this.currentState = state;
        this.currentState.Enter();
    }

    public void contactDamage(Collision2D collision)
    {
        if (!doContactDmg) return;
        if (collision.gameObject.tag == "Player")
        {
            Character chara = collision.collider.gameObject.GetComponent<Character>();
            if (chara != null && Time.time >= lastTickTime + cacTickTime)
            {
                chara.Damage(contact_damage, HpChangesType.normalDamages); // TODO: modifier en fonction du type de dégâts infligés
                this.lastTickTime = Time.time;
            }
        }
    }

    private void cleanTargets()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || !targets[i].gameObject.activeInHierarchy) targets.RemoveAt(i);
        }
    }

    public bool targetInRange(float range)
    {
        if (target != null)
        {
            return Vector2.Distance(collision_collider.transform.position, target.transform.position) <= range;

        }
        else return false;
    }

    public bool checkTargetInVision()
    {
        if (target == null) return false;
        string[] masks = { "Obstacle" };
        int mask = LayerMask.GetMask(masks);
        return !Physics2D.Linecast(collision_collider.transform.position, target.transform.position, mask);
    }

    public void registerPathManager(PathManager manager)
    {
        pathfinding.registerManager(manager);
    }

    public override void Die()
    {
        // Drops gold on death
        if (goldAmount > 0)
        {
            DropGold();
        }

        // Add score on death
        FindObjectOfType<AudioManager>().Play(SFX.MobDie);
        FindObjectOfType<ScoreManager>().AddWithMultiplier(scoreValue);

        PlayerPrefs.SetInt("enemies_numbers", PlayerPrefs.GetInt("enemies_numbers") - 1); // TODO: enlever ? On n'utilise plus les PlayerPrefs je crois

        Stats.killsCount++;

        base.Die();
    }

    private void DropGold()
    {
        GameObject gold = Instantiate(goldPrefab, transform.parent);
        gold.transform.position = transform.position;
        gold.GetComponent<Gold>().amount = goldAmount * (int)((float)PlayerPrefs.GetInt("Level") / 10f) + goldAmount;
        gold.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }
    public void OnDestroy()
    {
        GameData.enemies.Remove(this.gameObject);
    }

    public Vector2 targetDir()
    {
        return target == null ? Vector2.zero : (Vector2)target.transform.position - (Vector2)gameObject.transform.position;
    }

    public void Shoot(Vector2 dir)
    {
        this.weapon.ForceShoot(dir);
        FindObjectOfType<AudioManager>().Play(SFX.Shoot);
    }

    private void updateIsColliding()
    {
        string[] layers = { "Obstacle", "VoidObstacle" };
        int mask = LayerMask.GetMask(layers);
        this.isCollinding = collision_collider.IsTouchingLayers(mask);
    }


    public override void Damage(float value, HpChangesType damageType)
    {
        base.Damage(value, damageType);
        if (target == null)
        {
            foreach (GameObject player in Player.playerList)
            {
                if (player.activeInHierarchy && !targets.Contains(player.transform)) targets.Add(player.transform);
            }
        }
    }



}

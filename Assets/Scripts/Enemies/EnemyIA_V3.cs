using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA_V3 : Character {
    Vector2 vect_mouv;

    public float vision_range = 10f; //TODO à modifier une fois les tests finis
    public float esq_range = 1f;
    public float attack_range = 7f;

    public float wanderingSpeed = 0.5f;
    public float trackingSpeed = 2f;

    public int ray_count = 16;
    public float angle = 360;
    LayerMask obstacleMask;

    private float wanderingStartTime;
    private float timeToWander;

    bool collisionDetected;
    private enum States { IDLE, TRACKING, ATTACKING }
    private States currentState;

    List<Transform> targets;
    Transform target;

    public CircleCollider2D vision_collid;
    public CapsuleCollider2D capsule;

    public Pathfinding pathfinding;

    private Vector2 wanderingDirection;

    private Animator animator;

    public Weapon weapon;

    private bool attacking;

    private float cooltime = 2f;
    private float startCooltime = 0f;

    public int goldAmount = 0;
    public GameObject goldPrefab;

   

    public override void Start()
    {
        base.Start();

        this.animator = GetComponentInChildren<Animator>();
        targets = new List<Transform>();
        target = null;
        string[] masks = { "Obstacle", "VoidObstacle" };
        obstacleMask = LayerMask.GetMask(masks);
        currentState = States.IDLE;
        changeWanderingDirection();
        (data as CharacterData).speed = wanderingSpeed;
        vision_collid.radius = vision_range;
        capsule = GetComponent<CapsuleCollider2D>();
        this.pathfinding = GetComponent<Pathfinding>();
        this.attacking = false;
        GameData.enemies.Add(gameObject);
    }

    private void Animate()
    {
        float currentSpeed = rb.velocity.magnitude;
        animator.SetFloat("Speed", (Mathf.Max(currentSpeed - 0.1f, 0) / (data as CharacterData).speed));
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Animate();
        if (attacking) this.currentState = States.ATTACKING;
        switch (currentState)
        {
            case States.IDLE:
                attacking = false;
                weapon.StopShooting();
                (data as CharacterData).speed = wanderingSpeed;
                actIdle();
                break;

            case States.TRACKING:
                attacking = false;
                weapon.StopShooting();
                (data as CharacterData).speed = trackingSpeed;
                actTracking();
                break;

            case States.ATTACKING:
                actAttacking();
                break;
        }
    }




    void actIdle()
    {
        if (wanderingStartTime + timeToWander <= Time.time)
        {
            changeWanderingDirection();
        }
        Move(pathfinding.getMouvementByVector((Vector2)capsule.transform.position + wanderingDirection, capsule.transform.position).normalized);
    }

    void actTracking()
    {
        Vector2 direction = pathfinding.getMouvementVector();
        Move(direction);

        if (target != null && Vector2.Distance(capsule.transform.position, target.transform.position) < attack_range)
        {
            this.currentState = States.ATTACKING;
        }

        if (target == null)
        {
            this.currentState = States.IDLE;
        }
    }

    void actAttacking()
    {
        string[] masks = { "Obstacle" };
        
        Move(Vector2.zero);

        if (!attacking && cooltime + startCooltime <= Time.time)
        {
            Move(Vector2.zero);
            animator.SetTrigger("Attack");
            attacking = true;
        } else
        {
            this.currentState = States.TRACKING;
        }

        if (target != null && Physics2D.Raycast(gameObject.transform.position, target.transform.position - gameObject.transform.position, Vector2.Distance(gameObject.transform.position, target.transform.position), LayerMask.GetMask(masks)))
        {
            this.currentState = States.TRACKING;
            return;
        }
        if (target != null && Vector2.Distance(capsule.transform.position, target.transform.position) > attack_range)
        {
            this.currentState = States.TRACKING;
            return;
        }
        if (target == null)
        {
            this.currentState = States.IDLE;
            return;
        }
    }

    void changeWanderingDirection()
    {
        this.wanderingDirection = getRandomVect2();
        timeToWander = Random.Range(2f, 5f);
        wanderingStartTime = Time.time;
    }

    Vector2 getRandomVect2()
    {
        return Random.insideUnitCircle.normalized; ;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!targets.Contains(collision.gameObject.transform)) targets.Add(collision.gameObject.transform); //On détecte un joueur on se met à le poursuivre
            currentState = States.TRACKING;
            target = targets[Random.Range(0, targets.Count)];
            pathfinding.setTarget(target.transform);
            (data as CharacterData).speed = trackingSpeed;
        }
    }

    private void OnDestroy () {
        GameData.enemies.Remove(gameObject);
    }

    void changeTarget(Transform target)
    {
        this.target = target;
        pathfinding.setTarget(target);
    }
    public void registerPathManager(PathManager manager)
    {
        pathfinding.registerManager(manager);
    }

    public void Attack()
    {
        Move(Vector2.zero);
        Vector2 fire_dir = target.position - gameObject.transform.position;
        weapon.ShootDirection(fire_dir);
        weapon.StartShooting();
        attacking = false;
        startCooltime = Time.time;
    }
     
    public override void Die ()
    {
        // Drops gold on death
        GameObject gold = Instantiate(goldPrefab, transform.parent);
        gold.transform.position = transform.position;
        gold.GetComponent<Gold>().amount = goldAmount * (int)((float)PlayerPrefs.GetInt("Level") / 10f) + goldAmount;
        gold.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        PlayerPrefs.SetInt("enemies_numbers",PlayerPrefs.GetInt("enemies_numbers")-1);

        base.Die();
    }
}

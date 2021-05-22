using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA_V2 : Character
{
    // Start is called before the first frame update

    Vector2 vect_esq;
    Vector2 vect_objec;
    Vector2 vect_mouv;

    public float vision_range = 5f; //TODO à modifier une fois les tests finis
    public float esq_range = 1f;
    public float attack_range = 3f;

    public float wanderingSpeed = 0.5f;
    public float trackingSpeed = 2f;

    private float coeff_esq = 0.5f;
    private float coeff_objec = 0.5f;

    private float dist_objec;
    private float dist_esq;

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

    private Pathfinding_V1 pathfinder;
    private Pathfinding pathfinding;

    public override void Start()
    {
        base.Start();
        targets = new List<Transform>();
        target = null;
        string[] masks = { "Obstacle", "VoidObstacle" };
        obstacleMask = LayerMask.GetMask(masks);
        currentState = States.IDLE;
        vect_esq = Vector2.zero;
        changeWanderingDirection();
        (data as CharacterData).speed = wanderingSpeed;
        vision_collid.radius = vision_range;
        capsule = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //Debug.Log("Coeff esq " + coeff_esq + " Coeff obj " + coeff_objec);
        switch (currentState)
        {
            case States.IDLE:
                actIdle();
                break;

            case States.TRACKING:
                //Debug.Log("Je passe à l'attaque");
                actTracking();
                break;

            case States.ATTACKING:

                break;

            default:
                currentState = States.IDLE;
                break;
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < ray_count; i++)
        {
            var rotation = this.transform.rotation;
            var rotation_mod = Quaternion.AngleAxis((i / (float)ray_count) * angle, this.transform.forward);
            var direction = rotation * rotation_mod.normalized * Vector2.right * esq_range;
            Gizmos.color = Color.white;
            Gizmos.DrawRay(this.transform.position, direction);
            Gizmos.color = Color.red;
            Ray ray = new Ray(this.transform.position, vect_mouv * (data as CharacterData).speed);
            Gizmos.DrawRay(ray);
        }

        if (pathfinder != null)
        {
            Node nearest = pathfinder.getNearestNode(gameObject.transform.position);
            if (nearest != null) Gizmos.DrawSphere(new Vector2(nearest.x, nearest.y), 0.2f);
            /**
            List<Node> nodes = pathfinder.getValidNeighbours(nearest);
            foreach(Node node in nodes)
            {
                Gizmos.DrawSphere(new Vector2(node.x, node.y), 0.2f);
            }
            **/
        }
    }

    void calcVectMouv()
    {
        Vector2 res = Vector2.zero;
        bool flag = false;
        List<Vector2> noHitVectors = new List<Vector2>();
        for (int i = 0; i < ray_count; i++)
        {

            var rotation = this.transform.rotation;
            var rotation_mod = Quaternion.AngleAxis((i / (float)ray_count) * angle, this.transform.forward);
            var direction = rotation * rotation_mod * Vector2.right;
            RaycastHit2D hitInfo;

            if (hitInfo = Physics2D.CapsuleCast(capsule.transform.position, capsule.size * 0.95f, capsule.direction, capsule.transform.rotation.z, direction, esq_range, obstacleMask))
            {
                if (!flag) flag = true;
                float distToObst = hitInfo.distance;
                float coeff = (distToObst / esq_range);
                if (distToObst < dist_esq) dist_esq = distToObst;
            }
            else
            {
                noHitVectors.Add(direction);
            }
        }
        if (!flag)
        {
            dist_esq = esq_range;
        }


        if (noHitVectors.Count != 0)
        {
            float angleToObjec = Vector2.Angle(noHitVectors[0], vect_objec);
            res = noHitVectors[0];

            foreach (Vector2 noCollision in noHitVectors)
            {
                float angle = Vector2.Angle(noCollision, vect_objec);
                //Debug.Log("Angle " + angle);
                if (angle < angleToObjec)
                {
                    res = noCollision;
                    angleToObjec = angle;
                }
            }
        }
        coeff_esq = 1 - (dist_esq / esq_range);
        vect_mouv = res.normalized;
    }

    void actIdle()
    {
        if (wanderingStartTime + timeToWander <= Time.time)
        {
            changeWanderingDirection();
        }
        Move();
    }

    void changeWanderingDirection(Vector2? vector = null)
    {
        timeToWander = Random.Range(0f, 3f);
        wanderingStartTime = Time.time;
        if (vector != null)
        {
            vect_objec = (Vector2)vector;
        }
        else
        {
            vect_objec = getRandomVect2();
        }


    }

    void actTracking()
    {
        calcVectObjec();
        Move();
    }

    void calcVectObjec()
    {
        if (targets.Count != 0)
        {
            vect_objec = target.transform.position - gameObject.transform.position;
            vect_objec.Normalize();
            dist_objec = Vector2.Distance(gameObject.transform.position, target.position);
            coeff_objec = 1 - coeff_esq;
        }
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
            (data as CharacterData).speed = trackingSpeed;
        }
    }

    Vector2 getVectorMouv()
    {

        return vect_mouv;
    }

    void Move()
    {
        Vector2 newMouv;
        if (target != null)
        {
            newMouv = pathfinder.getMovingDirection(capsule.transform.position, target.position, capsule.bounds.size.x, capsule.bounds.size.y);
            if (newMouv == Vector2.zero)
            {
                calcVectMouv();
            }
            else
            {
                vect_mouv = newMouv;
            }
        }
        else calcVectMouv();
        Move(vect_mouv);
    }

    public void registerPathfinder(Pathfinding_V1 pathfinder)
    {
        this.pathfinder = pathfinder;
    }
}

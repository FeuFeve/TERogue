using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : Character
{

    Vector3 vect_esq = Vector3.zero;
    Vector3 vect_objec = Vector3.zero;
    Vector3 vect_mouv = Vector3.zero;
    Vector3 vect_ally = Vector3.zero;
    Vector3 vect_tir = Vector3.zero;

    public float vision_range = 10;
    public float esq_range = 2f;
    public float attack_range = 4f;

    //Les coefficients de poids pour chaque vecteur
    private float coeff_esq;
    private float coeff_obje;
    private float coeff_ally;

    //Les distances à l'objectif, à l'obstacle le plus proche et à l'allié le plus proche
    private float dist_obje;
    private float dist_esq;
    private float dist_ally;





    public CircleCollider2D vision_collid;
    private List<GameObject> obstacles = new List<GameObject>();
    GameObject target;
    public Weapon weapon;



    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        weapon = GetComponent<Weapon>();
        (data as CharacterData).maxHP = 50;
        HP = (data as CharacterData).maxHP;
        esq_range = esq_range + Vector3.Distance(transform.position, GetComponent<CapsuleCollider2D>().bounds.max);
        vision_collid.radius = vision_range;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        updateVectors();
        shoot();
        Move(vect_mouv);
        //Debug.Log("Vec_esq : " + vect_esq + " Coeff_esq : " + coeff_esq + " Dist_esq : " + dist_esq);
        //Debug.Log("Vec_obje : " + vect_objec + " Coeff_obje : " + coeff_obje + " Dist_obje : " + dist_obje);
    }

    void updateVectors()
    {
        calcVecTir();
        calcVecMouv();
    }

    void calcVecMouv()
    {
        calcVecAlly();
        calcVecObject();
        calcVecEsq();

        coeff_esq = (dist_esq < esq_range && dist_esq > 0) ? 1 - (dist_esq / esq_range) : 0;
        coeff_obje = (dist_obje < vision_range && dist_obje > attack_range) ? (dist_obje / vision_range) - coeff_esq : 0;

        vect_mouv = (vect_esq * coeff_esq) + (vect_objec * coeff_obje);
        vect_mouv.Normalize();
    }

    void calcVecTir()
    {
        GameObject nearest;
        float distance;
        float newDistance;
        List<GameObject> players = Player.playerList;
        if (players.Count != 0)
        {
            distance = Vector3.Distance(transform.position, players[0].transform.position);
            nearest = players[0];
            foreach (GameObject player in players)
            {
                newDistance = Vector3.Distance(transform.position, player.transform.position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = player;
                }
            }
            vect_tir = nearest.transform.position - transform.position;
            target = nearest;

        }
        else
        {
            vect_tir = Vector3.zero;
            target = null;
        }
        vect_tir.Normalize();
    }

    void calcVecAlly()
    {

    }

    void calcVecObject()
    {
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position); // La distance à l'objectif
            if (dist < attack_range)
            {
                vect_objec = Vector3.zero;
            }
            else
            {
                vect_objec = vect_tir;
                vect_objec.Normalize();
                dist_obje = dist;
            }
        }
        else
        {
            vect_objec = Vector3.zero;
            dist_obje = -1;
        }

    }
    void calcVecEsq()
    {
        float dist; //Distance à l'obstacle courant
        float minDist = -1;
        float coeff; //Coefficient du vecteur d'esquive en fonction de la distance à l'obstacle
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, vision_range, LayerMask.GetMask("Obstacle")); // Récupération des colliders dans la range de vision
        bool flag = true;
        Collider2D nearest = null;
        foreach (Collider2D obst in obstacles)
        {
            dist = Vector3.Distance(obst.transform.position, transform.position) + Vector3.Distance(obst.transform.position, obst.bounds.max);
            if (obst == GetComponent<Collider2D>() && dist <= Vector3.Distance(obst.transform.position, obst.bounds.max)) continue; //Si c'est un collider de l'ennemi on l'ignore

            if (flag)
            {
                minDist = dist;
                flag = !flag;
            }

            if (dist < minDist && dist > 0)
            {
                minDist = dist;
                nearest = obst;
            }
        }
        if (nearest != null)
        {
            vect_esq = -(nearest.transform.position - transform.position).normalized;
        }
        else
        {
            vect_esq = Vector3.zero;
        }
        dist_esq = minDist;
    }

    void shoot()
    {
        /**
        if (target == null)
        {
            weapon.StopShooting();
        }
        else
        {
            weapon.ShootDirection(vect_tir);
            weapon.StartShooting();
        }
        **/
    }
    //FIXEDUPDATE
}
;
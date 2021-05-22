using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSpark_IAController : IA_controller
{

    public GameObject toSummon;
    public override void Die()
    {
        System.Random random = new System.Random();
        for (int i = 0; i < 3; i++)
        {
            //Random.insideUnitCircle;
            Vector2 randVec = Random.insideUnitCircle * 1.3f;
            Vector2 spawnPos = this.pathfinding.getValidSpawnPos((Vector2)this.collision_collider.transform.position + this.collision_collider.offset + randVec);
            GameObject go = Object.Instantiate(this.toSummon, spawnPos, Quaternion.identity);
            Pathfinding toRegister = go.GetComponent<FlyingPathfinding>();
            if (toRegister != null) toRegister.registerManager(this.pathfinding.GetPathManager());
        }
        base.Die();
    }

    public override void Start()
    {
        base.Start();
        this.attack_state = new BigSpark_Attack_State(this);
    }
}

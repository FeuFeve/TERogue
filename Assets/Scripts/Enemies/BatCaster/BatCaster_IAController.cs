using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCaster_IAController : IA_controller
{

    public GameObject toSummon;
    [System.NonSerialized]
    public PathManager pathManager;


    public override void Start()
    {
        base.Start();
        this.pathManager = pathfinding.GetPathManager();
        this.attack_state = new BatCaster_Attack_State(this, toSummon);
    }

    public override void Animate()
    {
        base.Animate();
        float currentSpeed = rb.velocity.magnitude;
        animator.SetFloat("Speed", (Mathf.Max(currentSpeed - 0.1f, 0) / (data as CharacterData).speed));
    }
}

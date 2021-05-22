using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcBoss_Throw_Gourdin : Action
{
    private bool returning;
    private bool finished;
    private bool casting;

    Vector2 dir;

    private GameObject toThrow;

    private GameObject thrown;

    public float gourdinSpeed = 6f;

    public float followSpeed = 3f;

    public float flyTime = 1.5f;
    private float startTime;
    Vector3 oldDir;

    public OrcBoss_Throw_Gourdin(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        
        if (!casting && !returning && !finished) //Le gourdin est en phase d'aller
        {
            Rigidbody2D rb = thrown.GetComponent<Rigidbody2D>();
            rb.velocity = caller.controller.targetDir().normalized * gourdinSpeed * 1.5f;
        }

        if (!casting && returning && !finished) //Le gourdin est en phase de retour
        {
            Vector2 retDir = ((Vector2)caller.controller.collision_collider.transform.position + this.caller.controller.collision_collider.offset) - (Vector2)thrown.transform.position;
            Rigidbody2D rb = thrown.GetComponent<Rigidbody2D>();
            rb.velocity = retDir.normalized * gourdinSpeed *1.5f ;
        }

        if(!casting && !finished)
        {
            caller.controller.Move(caller.controller.pathfinding.getMouvementVector(), followSpeed);
        }

        if (!casting && !returning && flyTime + startTime <= Time.time)
        {
            returning = true;
        }

        if (returning && Vector2.Distance(thrown.transform.position, (Vector2)caller.controller.collision_collider.transform.position + this.caller.controller.collision_collider.offset) < 0.2f)
        {
            retrieveThrown();
        }
    }
    public override void Start()
    {
        base.Start();
        this.returning = false;
        this.finished = false;
        this.casting = true;
        OrcBoss_IAController cont = caller.controller as OrcBoss_IAController;
        this.toThrow = cont.gourdinToThrow;
        caller.controller.animator.SetTrigger("Attacking");
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        throwGourdin();
        this.casting = false;

    }

    private void throwGourdin()
    {
        dir = caller.controller.targetDir();
        this.startTime = Time.time;
        setActiveGourdin(false); //On cache le gourdin présent sur le sprite
        oldDir = dir;
        thrown = GameObject.Instantiate(toThrow, (Vector2)caller.controller.collision_collider.transform.position + this.caller.controller.collision_collider.offset, Quaternion.identity);
    }

    private void setActiveGourdin(bool val)
    {
        OrcBoss_IAController cont = caller.controller as OrcBoss_IAController;
        cont.gourdin.GetComponent<SpriteRenderer>().enabled = val;
    }

    private void retrieveThrown()
    {
        GameObject.Destroy(thrown);
        setActiveGourdin(true);
        End();
    }

}

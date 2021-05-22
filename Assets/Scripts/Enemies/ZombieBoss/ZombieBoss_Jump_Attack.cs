using UnityEngine;

internal class ZombieBoss_Jump_Attack : Action
{
    private bool jumping;
    private bool falling;
    private ScreenShaker shaker;
    private Vector2 pos;

    public ZombieBoss_Jump_Attack(IActionState caller, float cooltime) : base(caller, cooltime)
    {
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        Debug.Log("falling : " + falling + " jumping : " + jumping);
        if (jumping && !falling)
        {
            jumping = false;
            Vector2 pos = caller.controller.target != null ? caller.controller.pathfinding.getValidSpawnPos((Vector2)caller.controller.target.position) : (Vector2)caller.controller.gameObject.transform.position;
            Desactivate();
            caller.controller.gameObject.transform.position = pos;
            //TP le perso à la bonne position
        }
        else if (!jumping && !falling)
        {
            falling = true;
            Activate();
        }
        else if (falling)
        {
            //Tirer dans tous les sens
            int shots = 8;
            for (int i = 0; i < shots; i++)
            {

                var rotation = caller.controller.gameObject.transform.rotation;
                var rotation_mod = Quaternion.AngleAxis((i / (float)shots) * 360, caller.controller.gameObject.transform.forward);
                var direction = rotation * rotation_mod * Vector2.right;
                shaker.TriggerShake(0.8f);
                caller.controller.Shoot(direction);
                End();
            }
        }
    }

    public override void End()
    {
        base.End();
    }

    public override void Start()
    {
        base.Start();
        jumping = true;
        falling = false;
        this.shaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShaker>();
        caller.controller.animator.SetTrigger("Jump");
        //Mettre l'animateur sur l'anim de saut
    }

    private void Activate()
    {
        caller.controller.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        caller.controller.collision_collider.enabled = true;
    }
    private void Desactivate()
    {
        caller.controller.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        caller.controller.collision_collider.enabled = false;
    }
}
using UnityEngine;
internal class COD_Player_Snipe_Attack : Action
{
    public LineRenderer indicator;
    private bool casting;
    private Vector2 dir;
    public COD_Player_Snipe_Attack(IActionState caller, float cooltime, LineRenderer indicator) : base(caller, cooltime)
    {
        this.indicator = indicator;
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        if (casting)
        {
            indicator.SetPosition(0, caller.controller.gameObject.transform.position);

            if (caller.controller.target != null)
            {
                float singleStep = 0.4f * Time.deltaTime;
                dir = Vector3.RotateTowards(dir, caller.controller.targetDir(), singleStep, 0.0f);
                Debug.DrawLine(caller.controller.gameObject.transform.position, (Vector2)caller.controller.gameObject.transform.position + (dir * 100f));

                string[] masks = { "Obstacle" , "Players" };
                LayerMask mask = LayerMask.GetMask(masks);
                float maxDist = 12f;
                RaycastHit2D hit = Physics2D.Raycast(caller.controller.gameObject.transform.position, dir, maxDist);
                //Vector2 endPos = hit ? (Vector2) hit.point : (Vector2)caller.controller.gameObject.transform.position + (dir.normalized * maxDist);
                Vector2 endPos = (Vector2)caller.controller.gameObject.transform.position + (dir.normalized * maxDist);
                indicator.SetPosition(1, endPos);
            }
           
        }
    }


    public override void Start()
    {
        base.Start();
        indicator.enabled = true;
        indicator.SetPosition(0, caller.controller.gameObject.transform.position);
        indicator.enabled = true;
        this.casting = true;
        dir = caller.controller.targetDir();
        caller.controller.animator.SetTrigger("Attacking");
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        casting = false;
        caller.controller.Shoot(dir);
        End();
    }

    public override void End()
    {
        base.End();
        indicator.enabled = false;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcBoss_Swipe_Attack : Action
{
    ScreenShaker shaker;
    GameObject zone_indicator;
    Collider2D collider;
    float damage = 30f;
    private float startScaleX;
    private Vector2 lastDir;

    public OrcBoss_Swipe_Attack(IActionState caller, float cooltime, GameObject zoneIndicator) : base(caller, cooltime)
    {
        this.zone_indicator = zoneIndicator;
        this.startScaleX = zoneIndicator.transform.localScale.x;
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);

        /**
        Vector3 scale = zone_indicator.transform.localScale;
        scale.x = zone_indicator.transform.parent.localScale.x * Mathf.Abs(startScaleX);
        zone_indicator.transform.localScale = scale;
        //zone_indicator.transform.right = caller.controller.targetDir();//caller.controller.target.position - zone_indicator.transform.position;
        **/
        Transform target = caller.controller.target;
        Transform transform = zone_indicator.transform;
        // Determine which direction to rotate towards
        Vector3 targetDirection = caller.controller.targetDir();

        // The step size is equal to speed times frame time.
        float singleStep = 1.5f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.right, targetDirection, singleStep, 0.0f);

        transform.right = newDirection;

        lastDir = transform.right;
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
        shaker.TriggerShake(0.5f);

        Vector2 targDir = caller.controller.targetDir();
        caller.controller.Shoot(lastDir);
        string[] masks = { "Players" };
        int mask = LayerMask.GetMask(masks);
        Collider2D[] allOverlappingColliders = new Collider2D[16];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(mask);
        contactFilter.useLayerMask = true;

        int overlapCount = Physics2D.OverlapCollider(collider, contactFilter, allOverlappingColliders);

        GameObject.FindObjectOfType<AudioManager>().Play(SFX.OrcBossSmash);

        Collider2D current;
        for (int i = 0; i < overlapCount; i++)
        {
            current = allOverlappingColliders[i];
            if (current.gameObject.tag == "Player")
            {
                Character toDamage = current.GetComponent<Character>();
                if (toDamage != null)
                {
                    //TODO ajouter une force de déplacement
                    toDamage.Damage(damage, HpChangesType.criticalDamages);
                }
            }
        }
        //TODO faire les dégâts aux joueurs
        End();
    }

    public override void End()
    {
        base.End();
        zone_indicator.SetActive(false);
    }

    public override void Start()
    {
        base.Start();
        Vector3 scale = zone_indicator.transform.localScale;
        scale.x = zone_indicator.transform.parent.localScale.x * Mathf.Abs(startScaleX);
        zone_indicator.transform.localScale = scale;
        zone_indicator.transform.right = caller.controller.targetDir();
        zone_indicator.SetActive(true);
        this.shaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShaker>();
        this.collider = zone_indicator.GetComponent<Collider2D>();
        caller.controller.animator.SetTrigger("Swipe");
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneStack_IAController : IA_controller
{

    public int deathShots = 6;
    public override void Start()
    {
        base.Start();
        this.attack_state = new BoneStack_Attack_State(this);
    }

    public override void Die()
    {
        for (int i = 0; i < deathShots; i++)
        {

            var rotation = this.transform.rotation;
            var rotation_mod = Quaternion.AngleAxis((i / (float)deathShots) * 360, this.transform.forward);
            var direction = rotation * rotation_mod * Vector2.right;

            weapon.ForceShoot(direction);
        }
        base.Die();
    }
}

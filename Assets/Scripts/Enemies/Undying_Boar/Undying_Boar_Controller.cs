using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undying_Boar_Controller : IA_controller
{
    public Sprite first_skin;
    public Sprite second_skin;
    private SpriteRenderer renderer;

    private bool isUndead;
    private float startTime;
    public float reviveTime = 10f;

    private State p1_state;
    private State p2_state;
    public override void Start()
    {
        base.Start();
        this.p1_state = new Undying_Boar_Attack_State_P1(this);
        this.p2_state = new Undying_Boar_Attack_State_P2(this);
        this.attack_state = p1_state;
        this.renderer = GetComponentInChildren<SpriteRenderer>();
        isUndead = false;
    }

    public override void Update()
    {
        base.Update();
        if (startTime + reviveTime < Time.time && isUndead)
        {
            switchToAlive();
        }
    }


    public override void Die()
    {
        if(!isUndead)
        {
            switchToUndead();
        } else
        {
            base.Die();
        }
    }

    private void switchToUndead()
    {
        isUndead = true;
        renderer.sprite = second_skin;
        HP = (data as CharacterData).maxHP / 2;
        startTime = Time.time;
        this.attack_state = p2_state;
        changeState(tracking_state);
    }

    private void switchToAlive()
    {
        isUndead = false;
        renderer.sprite = first_skin;
        this.HP = (data as CharacterData).maxHP;
        this.attack_state = p1_state;
        changeState(tracking_state);
    }
}

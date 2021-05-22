using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_eat_attack : Action
{
    private float move_speed; //Vitesse de déplacement pendant l'attaque
    private float duration = 5f; //Durée en secondes de la phase
    private float startTime; //Le début de l'attaque
    public bool eating;
    public Mimic_eat_attack(IActionState caller, float cooltime) : base(caller, cooltime)
    {
        this.move_speed = caller.controller.tracking_speed * 1.10f; //On augmente la vitesse de track de 10%
    }

    public override void Act(State attackState)
    {
        base.Act(attackState);
        caller.controller.Move(caller.controller.pathfinding.getMouvementVector(), move_speed);
        if (duration + startTime <= Time.time)
        {
            End();
        }
    }

    public override void animationTriggerIsCalled()
    {
        base.animationTriggerIsCalled();
    }

    public override void End()
    {
        base.End();
        eating = false;
    }

    public override void Start()
    {
        base.Start();
        this.startTime = Time.time;
        eating = true;
    }
}

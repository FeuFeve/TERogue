using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public IA_controller controller;

    public State(IA_controller controller)
    {
        this.controller = controller;
    }

    public virtual void Act(IA_controller controller) { }


    public virtual void animationTriggerIsCalled(IA_controller controller)
    {
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicUpdate()
    {

    }
}

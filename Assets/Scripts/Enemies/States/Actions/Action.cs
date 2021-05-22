using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    protected IActionState caller;
    public bool isRunning;
    public float cooltime;

    protected Action(IActionState caller, float cooltime)
    {
        this.caller = caller;
        isRunning = false;
        this.cooltime = cooltime;
    }

    public virtual void Act(State attackState) { }

    public virtual void Start() {
        isRunning = true;
    }

    public virtual void End() {
        isRunning = false;
        caller.endAction(this);
    }

    public virtual void animationTriggerIsCalled() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IActionState : State
{
    public Action currentAction;
    public List<Action> actions;
    private Dictionary<Action, float> cooltimes;

    private float cd_start_time;
    private bool canLaunchAttack;

    private int mouvDir = 1;
    private float startTime;
    private float changeDuration = 2f;
    public IActionState(IA_controller controller) : base(controller)
    {
        this.actions = new List<Action>();
        this.cooltimes = new Dictionary<Action, float>();
        foreach (Action action in actions)
        {
            cooltimes[action] = -float.MaxValue;
        }
    }
    public override void Act(IA_controller controller)
    {
        base.Act(controller);

        if (currentAction != null && currentAction.isRunning) { 
            currentAction.Act(this); 
        }
        else
        {
            if (startTime + changeDuration <= Time.time)
            {
                mouvDir = Random.Range(0, 99) < 50 ? -1 : 1;
                changeDuration = Random.Range(1, 3);
                startTime = Time.time;
            }
            Vector2 dir = controller.targetDir();
            Vector2 trueDir = Vector2.Perpendicular(dir).normalized * mouvDir;
            controller.Move(trueDir, controller.wandering_speed);
        }
    }

    public override void animationTriggerIsCalled(IA_controller controller)
    {
        base.animationTriggerIsCalled(controller);
    }

    public override void Enter()
    {
        base.Enter();
        foreach (Action action in actions)
        {
            cooltimes[action] = Time.time - Random.Range(0, action.cooltime);
        }
        canLaunchAttack = true;
        launchAction();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        canLaunchAttack = currentAction == null || !currentAction.isRunning;
        if (canLaunchAttack) launchAction();
        //Debug.Log("Action can be started : " + (!action_is_active && cooldown_time + cd_start_time <= Time.time) + "; action is active : " + action_is_active + " " + (cooldown_time + cd_start_time) + " " + Time.time);
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
    }

    public void launchAction()
    {
        List<Action> valid = new List<Action>();
        foreach (Action action in actions)
        {
            float actionStartTime;
            cooltimes.TryGetValue(action, out actionStartTime);
            if (action.cooltime + actionStartTime <= Time.time)
            {
                valid.Add(action);
            }
        }
        if (this.currentAction != null) currentAction.End();
        this.currentAction = valid.Count != 0 ? valid[Random.Range(0, valid.Count)] : null;
        if (this.currentAction != null) this.currentAction.Start();
    }

    public void endAction(Action action)
    {
        cooltimes[action] = Time.time;
    }
}

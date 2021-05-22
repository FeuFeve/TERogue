using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnEffect : Powerup {
    public int stacks = 1;
    public float chancesPerStack = 0.1f;
    public StatusEffect ApplyOnTarget = null;
    public StatusEffect ApplyOnCaster = null;
    
    public enum GrowthType {
        Linear,
        Hyperbolic
    };
    public GrowthType growth;

    public virtual void AddStack () {
        stacks++;
    }

    public virtual bool Roll () {
        if (growth == GrowthType.Linear)
            return Random.Range(0.0f, 1.0f) < stacks * chancesPerStack;
        else
            return Random.Range(0.0f, Mathf.PI / 2f) < Mathf.Atan(stacks * chancesPerStack);
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        base.OnTriggerEnter2D(collider);
    }
}

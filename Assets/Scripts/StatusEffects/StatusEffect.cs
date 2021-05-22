using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour {
    protected int stacks = 1; public int Stacks { get { return stacks; } }
    private float expirationTime = 0;
    private bool firstApply = true;

    protected float duration = 1;
    protected float tickEvery = 0.25f;
    protected bool ResetDurationOnApply = true;
    protected bool AddStackOnApply = false;

    public GameObject caster;

    /**
     *  <summary>Called when the effect is applied, handles the stacks and the timer</summary> 
     *  <remarks>when overriding, call base.OnApply() at the start</remarks>
     */
    public virtual void OnApply () {
        //PAS BEAU
        Weapon w = GetComponent<Weapon>();
        if (w != null) {
            w.StopCoroutine("shootingCoroutine");
            w.StartCoroutine("shootingCoroutine");
        }

        if (ResetDurationOnApply || firstApply) {
            expirationTime = Time.time + duration;
        }

        if (AddStackOnApply && !firstApply) stacks++;
        
        firstApply = false;
    }

    /**
     * <summary>Called when the status expires</summary>
     * <remarks>when overriding, call base.OnExpire() at then end</remarks>
     */
    protected virtual void OnExpire () {
        StopCoroutine("TickCoroutine");

        Destroy(this);
    }
     
    /**
     * <summary>Called when the status begins</summary>
     * <remarks>when overriding, call base.OnBegin() at then end</remarks>
     */
    protected virtual void OnBegin () {
        OnApply();
        
        StartCoroutine("TickCoroutine");
    }

    /**
     * <summary>tick coroutine, it calls OnTick</summary>
     */
    private IEnumerator TickCoroutine () {
        while (true) {
            if (expirationTime > Time.time) {
                yield return new WaitForSeconds(tickEvery);
                OnTick();
            }
            else
                break;
        }
        OnExpire();
    }

    protected virtual void OnTick () {

    }

    // Start is called before the first frame update
    void Awake () {
        Debug.Log("Status effect started !");
        OnBegin();
    }
}

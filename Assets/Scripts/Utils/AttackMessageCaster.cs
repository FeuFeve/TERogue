using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMessageCaster : MonoBehaviour
{
    // Start is called before the first frame update
    private IA_controller script;
    void Start()
    {
        this.script = GetComponentInParent<IA_controller>();
    }

    public void animationTriggerIsCalled()
    {
        script.animationTriggerIsCalled();
    }
}

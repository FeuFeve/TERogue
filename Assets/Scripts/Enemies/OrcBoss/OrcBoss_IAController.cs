using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcBoss_IAController : IA_controller
{

    public GameObject zoneIndicator;
    public GameObject gourdin;
    public GameObject gourdinToThrow;

    public override void Start()
    {
        base.Start();
        zoneIndicator.SetActive(false);
        this.attack_state = new OrcBoss_Attack_State(this, zoneIndicator);
    }
}

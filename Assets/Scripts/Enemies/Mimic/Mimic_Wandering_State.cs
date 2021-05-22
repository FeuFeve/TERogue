using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Wandering_State : State
{
    public Mimic_Wandering_State(IA_controller controller) : base(controller)
    {
        controller.wandering_speed = 0;
    }
}

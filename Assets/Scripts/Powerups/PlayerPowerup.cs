using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerup : Powerup {
    public Character.CharacterData add;
    public Character.CharacterData mul;
    public Character.CharacterData set;

    void OnTriggerEnter2D (Collider2D collider) {
        Character c = collider.gameObject.GetComponent<Character>();
        if (c == null) return;

        base.OnTriggerEnter2D(collider);

        Character.CharacterData cd = collider.gameObject.GetComponent<Character>().data as Character.CharacterData;
        cd.Add(add);
        cd.Mul(mul);
        cd.Set(set);
        //Debug.Log(cd.maxHP + " " + cd.maxShield + " " + cd.speed + " " + cd.evasion);
        PrintInfo();
    }
}

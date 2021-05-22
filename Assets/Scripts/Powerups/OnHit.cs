using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHit : OnEffect {

    void OnTriggerEnter2D (Collider2D collider) {
        Character c = collider.gameObject.GetComponent<Character>();
        if (c == null) return;

        base.OnTriggerEnter2D(collider);

        c.AddOnHit(this);
        PrintInfo();
    }
}

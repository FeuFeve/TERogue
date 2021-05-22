using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killAfterDelay : MonoBehaviour
{
    public float delay = 1;

    void Start() {
        Invoke("die", delay);
    }

    public void die () {
        Destroy(this.gameObject);
    }
}

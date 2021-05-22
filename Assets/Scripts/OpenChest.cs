using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour {

    private Animator anim;
    private bool opened = false;

    public List<GameObject> loots;


    public virtual void Start() {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {

            if (!opened) {
                AudioManager.instance.Play(SFX.OpenChest);
                anim.SetTrigger("Open_chest");
                BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
                foreach (BoxCollider2D collider in colliders) {
                    if (collider.isTrigger) collider.enabled = false;
                }
            }
            opened = true;
        }
    }

    public void DropItem() {
        var powerUp = Instantiate(loots[Random.Range(0, loots.Count)], transform.position, transform.rotation);
        powerUp.transform.parent = transform; // Put the power-up as a child of the chest so it is not instantiated in the Persistant scene
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders) {
            collider.enabled = false;
        }
    }
}

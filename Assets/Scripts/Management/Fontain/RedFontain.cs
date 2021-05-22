using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedFontain : MonoBehaviour
{
    public Animator animator;
    private bool isEmpty = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isEmpty)
            {
                collision.gameObject.GetComponent<Character>().
                    Heal((collision.gameObject.GetComponent<Character>().data as Character.CharacterData).maxHP * 0.3f
                        ,HpChangesType.normalHeal);
                animator.SetTrigger("Empty");
                isEmpty = true;
            }
    }
}

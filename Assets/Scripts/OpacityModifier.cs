using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityModifier : MonoBehaviour
{
    public float alpha = 0.5f;
    SpriteRenderer renderer;
    Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInParent<SpriteRenderer>();
        baseColor = renderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            renderer.color = new Color(1, 1, 1, alpha);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            renderer.color = baseColor;
        }
    }
}

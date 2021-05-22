using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HitboxHelper : MonoBehaviour
{
    static float ratio = 3f/4f;
    [SerializeField] int width = 1;
    [SerializeField] int height = 1;
    [SerializeField] int depth = 1;
    [SerializeField] int altitude = 1;
    [SerializeField] bool colliderChoice = true;
 
    [SerializeField] string state = "capsule collider";

    [SerializeField] bool submit = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnSubmit() {
        submit = false;
        float widthR = width * 0.1f;
        float depthR = depth * ratio * 0.1f;

        DestroyImmediate(transform.GetComponent<BoxCollider2D>());
        DestroyImmediate(transform.GetComponent<CircleCollider2D>());
        DestroyImmediate(transform.GetComponent<CapsuleCollider2D>());

        if (colliderChoice) {
            CapsuleCollider2D cc = transform.gameObject.AddComponent<CapsuleCollider2D>();
            if (widthR > depthR) cc.direction = CapsuleDirection2D.Horizontal;
            else cc.direction = CapsuleDirection2D.Vertical;
            cc.size = new Vector2(widthR, depthR);
        } else {
            BoxCollider2D bc = transform.gameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(widthR, depthR);
        }
    }
 
    // F   E
    // B   A
    // C L D
    // I   J
    //   K 
    // G   H
    void OnDrawGizmosSelected() {
        if (!enabled) return;

        if (width < 0) width = 0;
        if (height < 0) height = 0;
        if (depth < 0) depth = 0;
        if (altitude < 0) altitude = 0;

        if (colliderChoice) state = "capsule collider";
        else state = "box collider";

        if (submit) OnSubmit();

        float widthR = width * 0.1f;
        float heightR = height * 0.1f;
        float altitudeR = altitude * 0.1f;
        float depthR = depth * ratio * 0.1f;
        Vector3 origin = transform.position - new Vector3(0, depthR / 2, 0);
        
        Vector3 A = origin + new Vector3(widthR / 2, heightR + altitudeR, 0);
        Vector3 B = A - new Vector3(widthR, 0, 0);
        Vector3 C = B - new Vector3(0, heightR, 0);
        Vector3 D = C + new Vector3(widthR, 0, 0);

        Vector3 E = A + new Vector3(0, depthR, 0);
        Vector3 F = B + new Vector3(0, depthR, 0);

        Vector3 G = origin - new Vector3(widthR / 2, 0, 0);
        Vector3 H = origin + new Vector3(widthR / 2, 0, 0);

        Vector3 I = G + new Vector3(0, depthR, 0);
        Vector3 J = H + new Vector3(0, depthR, 0);

        Vector3 K = G + ((H - G) / 2) + ((I - G) / 2);
        Vector3 L = C + ((D - C) / 2);


        Gizmos.color = new Color(1f, .5f, .2f);

        Gizmos.DrawLine(I, G);
        Gizmos.DrawLine(J, H);
        Gizmos.DrawLine(H, G);

        if (I.y < C.y)
            Gizmos.DrawLine(I, J);
        if (K.y < L.y)
            Gizmos.DrawLine(K, L);

        Gizmos.color = new Color(.2f, 1, .2f);

        Gizmos.DrawLine(A, B);
        Gizmos.DrawLine(B, C);
        Gizmos.DrawLine(C, D);
        Gizmos.DrawLine(D, A);

        Gizmos.DrawLine(A, E);
        Gizmos.DrawLine(E, F);
        Gizmos.DrawLine(F, B);
    }
}

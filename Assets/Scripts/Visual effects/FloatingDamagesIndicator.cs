using UnityEngine;

public class FloatingDamagesIndicator : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 1f);

        float xOffset = Random.Range(-0.2f, 0.5f);
        float yOffset = 0.8f + Random.Range(0f, 0.5f);
        transform.localPosition += new Vector3(xOffset, yOffset, 0);
    }
}

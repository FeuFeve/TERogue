using UnityEngine;

public class CanvasResizer : MonoBehaviour {

    float currentAspect = 16f / 9f;

    private void Awake() {
        ResizeIfNeeded();
    }

    private void Update() {
        ResizeIfNeeded();
    }

    private void ResizeIfNeeded() {
        float cameraAspect = Camera.main.aspect;
        if (Mathf.Round(cameraAspect * 100f) / 100f != Mathf.Round(currentAspect * 100f) / 100f) {
            float newRatio = cameraAspect / currentAspect;

            RectTransform rt = transform.GetComponent<RectTransform>();
            rt.localScale = new Vector3(rt.localScale.x * newRatio, rt.localScale.y * newRatio, rt.localScale.z * newRatio);

            currentAspect = cameraAspect;
        }
    }
}

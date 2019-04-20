using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public float zoom;

    private Bounds target;
    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
        UpdateBounds();
    }

    private void LateUpdate() {
        //if (!GameManager.instance.bounds.Equals(target))
            UpdateBounds();
    }

    private void UpdateBounds () {
        if (GameManager.instance == null) return;

        target = GameManager.instance.bounds;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = target.size.x / target.size.y;
        float inversedTargetRatio = target.size.y / target.size.x;

        if (screenRatio >= targetRatio) {
            cam.orthographicSize = target.size.y / 2;
        } else {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = target.size.y / 2 * differenceInSize;
        }

        Vector3 newPos = transform.position;
        newPos.x = target.center.x;
        newPos.y = target.center.y;
        transform.position = newPos + offset;
    }
}

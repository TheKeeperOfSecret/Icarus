using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraScaler : MonoBehaviour
{
    public float targetHeight = 9f;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        camera.orthographicSize = targetHeight / (2f * camera.aspect);
    }
}

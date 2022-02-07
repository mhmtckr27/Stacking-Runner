using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    private Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Start()
    {
        target = PlayerController.Instance.transform;
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        smoothedPosition.z = desiredPosition.z;
        transform.position = smoothedPosition;
    }
}

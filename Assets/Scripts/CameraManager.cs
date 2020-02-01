using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    public float distance = 50;

    public float advanceMult = 2;
    public float maxRange = 50;

    Vector3 followPosition;
    Vector3 followPositionSmoothVelo;

    Rigidbody targetRb;

    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!target) return;

        Vector3 targetVelocity = targetRb.velocity;
        targetVelocity.y = 0;

        Vector3 addedVelocity = targetVelocity * advanceMult;
        addedVelocity = Vector3.ClampMagnitude(addedVelocity, maxRange);
        Vector3 targetPosition = target.position + addedVelocity;

        followPosition = Vector3.SmoothDamp(followPosition, targetPosition, ref followPositionSmoothVelo, 1);

        transform.position = followPosition - transform.forward * distance;
    }
}

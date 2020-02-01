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

    public bool FollowAdditionalTargets;
    public List<Transform> multipleTargets;
    
    List<Rigidbody> targetRbs;
    Rigidbody targetRb;

    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody>();
        if (multipleTargets != null)
        {
            targetRbs = new List<Rigidbody>();
            for (int i = 0; i < multipleTargets.Count; i++)
            {
                targetRbs.Add(multipleTargets[i].GetComponent<Rigidbody>());
            }
        }
    }

    void Update()
    {
        if (!target) return;

        Vector3 targetVelocity = targetRb.velocity;
        targetVelocity.y = 0;

        Vector3 addedVelocity = targetVelocity * advanceMult;
        addedVelocity = Vector3.ClampMagnitude(addedVelocity, maxRange);
        if (FollowAdditionalTargets && targetRbs != null)
        {
            for (int i = 0; i < targetRbs.Count; i++)
            {
                var velocity = targetRbs[i].velocity;
                velocity.y = 0;
                addedVelocity += Vector3.ClampMagnitude(velocity * advanceMult, maxRange);
            }
            addedVelocity /= targetRbs.Count + 1;
        }
        
        Vector3 targetPosition = target.position + addedVelocity;
        if (FollowAdditionalTargets && multipleTargets != null)
        {
            for (int i = 0; i < multipleTargets.Count; i++)
            {
                targetPosition += multipleTargets[i].position;
            }
            targetPosition /= multipleTargets.Count + 1;
        }

        followPosition = Vector3.SmoothDamp(followPosition, targetPosition, ref followPositionSmoothVelo, 1);

        transform.position = followPosition - transform.forward * distance;
    }
}

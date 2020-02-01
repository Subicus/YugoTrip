using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTransform : MonoBehaviour
{
    WheelCollider wheel;
    public Transform graphicalWheel;

    private void Start()
    {
        wheel = GetComponent<WheelCollider>();
    }

    void Update()
    {
        Vector3 pos;
        Quaternion rot;
        wheel.GetWorldPose(out pos, out rot);

        graphicalWheel.transform.position = pos;
        graphicalWheel.transform.rotation = rot;
    }
}

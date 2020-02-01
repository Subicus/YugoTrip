using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoMSetter : MonoBehaviour
{
    public Transform com;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = com.localPosition;
    }
}

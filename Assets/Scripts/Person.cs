using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    
    public float speed;
    public bool isFirstPlayer;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float h = Input.GetAxis(isFirstPlayer ? "Horizontal" : "Horizontal2");
        float v = Input.GetAxis(isFirstPlayer ? "Vertical" : "Vertical2");
        
        // Move player relative to camera, but projected to ground
        Vector3 inputV = Vector3.ClampMagnitude(new Vector3(h, v), 1);
        Transform camT = Camera.main.transform;
        Vector3 camRelativeV = camT.right * inputV.x + camT.up * inputV.y;
        
        Vector3 targetPosition = transform.position + camRelativeV;
        targetPosition.y = 0;
        
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = new Ray(camRelativeV, camT.forward);

        float enter;
        groundPlane.Raycast(ray, out enter);
        Vector3 forceV = ray.GetPoint(enter) * speed;

        rb.AddForce(forceV);
    }
}

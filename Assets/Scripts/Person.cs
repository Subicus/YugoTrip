﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    
    public float speed;
    public bool isFirstPlayer;
    public float minDistanceToYugo;
    
    private Rigidbody rb;

    private BrokenPart closestBrokenPart;
    private BrokenPart takenBrokenPart;
    private Driver yugo;
    private BrokenPartsManager partsManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yugo = FindObjectOfType<Driver>();
        partsManager = FindObjectOfType<BrokenPartsManager>();
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

        if (Input.GetKeyDown(isFirstPlayer ? KeyCode.Space : KeyCode.Return))
        {
            if (takenBrokenPart != null)
            {
                var distanceToYugo = Vector3.Distance(transform.position, yugo.transform.position);
                if (distanceToYugo <= minDistanceToYugo)
                {
                    partsManager.RepairWithPart(takenBrokenPart);
                    takenBrokenPart = null;
                    // TODO: repair Yugo!
                }
                else
                {
                    closestBrokenPart = takenBrokenPart;
                    takenBrokenPart.SetTaken(null);
                    takenBrokenPart = null;
                }
            }
            else if (closestBrokenPart != null)
            {
                takenBrokenPart = closestBrokenPart;
                closestBrokenPart.SetTaken(transform);
                closestBrokenPart = null;
            }
        }
    }

    public void GoOutOfCar()
    {
        if (isFirstPlayer)
        {
            transform.position = yugo.transform.position + Vector3.left * 2f;
        }
        else
        {
            transform.position = yugo.transform.position + Vector3.right * 2f;
        }
        gameObject.SetActive(true);
    }

    public void GoInCar()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("BrokenPart"))
            return;
        
        // it can be added
        if (closestBrokenPart != null)
        {
            closestBrokenPart.SetSelected(false, isFirstPlayer);
        }
        closestBrokenPart = other.gameObject.GetComponentInParent<BrokenPart>();
        closestBrokenPart.SetSelected(true, isFirstPlayer);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("BrokenPart"))
            return;
        
        if (closestBrokenPart != null)
        {
            closestBrokenPart.SetSelected(false, isFirstPlayer);
        }
        closestBrokenPart = null;
    }
}

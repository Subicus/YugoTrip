using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    
    public float speed;
    public bool isFirstPlayer;
    public float minDistanceToYugo;

    public GameObject on;
    public GameObject ona;
    
    private Rigidbody rb;

    private BrokenPart closestBrokenPart;
    private BrokenPart takenBrokenPart;
    private Driver yugo;
    private BrokenPartsManager partsManager;
    
    public bool IsInCar { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yugo = FindObjectOfType<Driver>();
        partsManager = FindObjectOfType<BrokenPartsManager>();
    }

    private void Start()
    {
        on.SetActive(isFirstPlayer);
        ona.SetActive(!isFirstPlayer);
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
        Vector3 forceV = ray.GetPoint(enter) * (speed * Time.deltaTime);

        rb.AddForce(forceV);

        
        var isSpace = isFirstPlayer ?
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) 
            : (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick2Button1));
        if (isSpace)
        {
            var distanceToYugo = Vector3.Distance(transform.position, yugo.transform.position);
            if (GameManager.I.IsRepairing)
            {
                if (takenBrokenPart != null)
                {
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
            else if (GameManager.I.IsRunning)
            {
                if (distanceToYugo <= minDistanceToYugo)
                {
                    GoInCar();
                }
            }
        }
    }

    public void GoOutOfCar()
    {
        IsInCar = false;
        var forward = yugo.transform.forward.normalized;
        if (isFirstPlayer)
        {
            // left side
            transform.position = yugo.transform.position - new Vector3(forward.z, forward.y, -forward.x) * 2f + Vector3.up;
        }
        else
        {
            // right side
            transform.position = yugo.transform.position + new Vector3(forward.z, forward.y, -forward.x) * 2f + Vector3.up;
        }
        gameObject.SetActive(true);
    }

    public void GoInCar()
    {
        IsInCar = true;
        gameObject.SetActive(false);
        GameManager.I.GoInCar();
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

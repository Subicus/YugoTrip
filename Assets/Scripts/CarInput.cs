using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInput : MonoBehaviour
{
    public CarController controller;

    private void Update()
    {
        controller.accelInput = Input.GetAxis("Vertical");
        controller.steerInput = Input.GetAxis("Horizontal");

    }
}

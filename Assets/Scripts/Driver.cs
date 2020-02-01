using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    public WheelCollider[] poweredWheels;
    public WheelCollider[] steeredWheels;
    public WheelCollider[] handbrakeWheels;

    public float maxAccelTorque = 10000;
    public float maxBrakeTorque = 10000;
    public float maxhandbrakeTorque = 10000;
    public float maxSteer = 30;

    public float maxSpeed = 30;

    Rigidbody rb;

    public Material brakeLightMaterial;
    public Material reverseLightMaterial;

    public ParticleSystem smokeParticleSystem;
    public ParticleSystem explosionParticleSystem;
    public ParticleSystem fireParticleSystem;
    public GameObject[] carObjects;
    public Material sasijaMaterijal;
    private Color initialSasijaMaterijalColor;

    private float health;
    public float StartHealth;
    public float HealthDecreasePerSecond;
    public float HealthDecreaseRoughPerSecond;

    int emissionId;
    public bool IsBroken { get; set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        emissionId = Shader.PropertyToID("_EmissionColor");
        smokeParticleSystem.Stop();

        health = StartHealth;
        if (GameManager.I == null)
        {
            HealthDecreasePerSecond = HealthDecreaseRoughPerSecond = 0;
        }

        initialSasijaMaterijalColor = sasijaMaterijal.color;
    }

    private void OnDestroy()
    {
        sasijaMaterijal.color = initialSasijaMaterijalColor;
    }

    void Update()
    {
        var isDriving = true;
        if (GameManager.I != null)
        {
            isDriving = GameManager.I.IsDriving;
        }
        if (isDriving)
        {
            UpdateHealth();    
        }
        
        float accelInput = isDriving ? Input.GetAxis("Vertical") : 0;
        float steerInput = isDriving ? Input.GetAxis("Horizontal") : 0;

        float handbrakeInput = isDriving && Input.GetKey(KeyCode.Space) ? 1 : 0;

        float accel = 0;
        float brake = 0;

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        float forwardVelo = localVelocity.z;

        float brakeLightIntensity = 0;
        float reverseLightIntensity = 0;

        if (forwardVelo < 0.01f && forwardVelo > -0.01f)
        {
            accel = accelInput;
        }
        else if (forwardVelo > 0.01f)
        {
            accel = Mathf.Clamp01(accelInput);
            brake = Mathf.Clamp01(-accelInput);
        }
        else
        {
            accel = Mathf.Clamp01(-accelInput);
            brake = Mathf.Clamp01(accelInput);

            reverseLightIntensity = 1;
        }

        if (rb.velocity.magnitude > maxSpeed)
            accelInput = 0;
        
        if (IsBroken)
        {
            brake = 1;
        }
        
        if (brake > 0.5f)
            brakeLightIntensity = 1;

        for (int i = 0; i < poweredWheels.Length; i++)
        {
            poweredWheels[i].motorTorque = accelInput * maxAccelTorque;
            poweredWheels[i].brakeTorque = brake * maxBrakeTorque;
        }

        for (int i = 0; i < steeredWheels.Length; i++)
        {
            steeredWheels[i].steerAngle = steerInput * maxSteer;
        }

        for (int i = 0; i < handbrakeWheels.Length; i++)
        {
            handbrakeWheels[i].brakeTorque = handbrakeInput * maxhandbrakeTorque;
            var fric = handbrakeWheels[i].sidewaysFriction;
            fric.stiffness = 1 - handbrakeInput * 0.8f;
            handbrakeWheels[i].sidewaysFriction = fric;
        }

        //Debug.DrawRay(transform.position, rb.velocity * 1, Color.yellow);

        if (brakeLightMaterial)
        {
            Color brakeColor = new Color(brakeLightIntensity * 2, 0, 0, 1);
            brakeLightMaterial.SetColor(emissionId, brakeColor);
        }

        if (reverseLightMaterial)
        {
            float v = reverseLightIntensity * 2;
            Color reverseColor = new Color(v, v, v, 1);
            reverseLightMaterial.SetColor(emissionId, reverseColor);
        }
    }

    public void Break()
    {
        IsBroken = true;
        smokeParticleSystem.Play();
    }

    public void Repair()
    {
        health = StartHealth;
        IsBroken = false;
        smokeParticleSystem.Stop();
    }

    public void Explode()
    {
        StartCoroutine(DoExplosion());
    }

    private IEnumerator DoExplosion()
    {
        IsBroken = true;
        explosionParticleSystem.Play();
        yield return new WaitForSeconds(0.25f);
        fireParticleSystem.Play();
        for (int i = 0; i < carObjects.Length; i++)
        {
            carObjects[i].SetActive(false);
        }
        sasijaMaterijal.color = Color.black;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            if (GameManager.I != null)
            {
                GameManager.I.ExplodeCar(); 
            }
            else
            {
                Explode();
            }
        }
    }

    private void UpdateHealth()
    {
        if (health < 0)
            return;
        
        health -= Time.deltaTime * HealthDecreasePerSecond;
        if (health <= 0)
        {
            health = 0f;
            GameManager.I.BreakCar();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public float HealthDecreaseInitial;

    public Text cloudText;
    private CanvasGroup cloudCanvasGroup;
    public Transform worldCanvas;
    private IEnumerator cloudAnimation;

    private bool wasBrokenOnce;
    private int initialFontSize;

    int emissionId;
    public bool IsBroken { get; private set; }
    public bool IsEmpty { get; private set; }

    private void Start()
    {
        initialFontSize = cloudText.fontSize;
        rb = GetComponent<Rigidbody>();
        cloudCanvasGroup = cloudText.GetComponent<CanvasGroup>();
        cloudCanvasGroup.alpha = 0f;

        emissionId = Shader.PropertyToID("_EmissionColor");
        smokeParticleSystem.Stop();

        health = StartHealth;
        if (GameManager.I == null)
        {
            HealthDecreasePerSecond = HealthDecreaseInitial = 0;
        }

        initialSasijaMaterijalColor = sasijaMaterijal.color;

        ShowCloud("LET'S GO!", 3f, 2f);
    }

    private void OnDestroy()
    {
        sasijaMaterijal.color = initialSasijaMaterijalColor;
        StopAllCoroutines();
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

        var isSpace = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton1);
        float handbrakeInput = isDriving && isSpace ? 1 : 0;

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
        
        if (IsBroken || IsEmpty)
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
        
        // refresh the ui depending on camera
        worldCanvas.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }

    public void StartBreaking()
    {
        smokeParticleSystem.Play();
    }

    public void Break()
    {
        IsBroken = true;
        wasBrokenOnce = true;
        ShowCloud("REPAIR YUGO!");
    }

    public void EmptyOut()
    {
        IsEmpty = true;
        ShowCloud("LET'S TAKE A BREAK!");
    }

    public void Repair()
    {
        health = StartHealth;
        IsBroken = false;
        smokeParticleSystem.Stop();

        var repairedStrings = new List<string>
        {
            "STILL THE BEST CAR",
            "AMAZING CAR!",
            "YUGO IS THE BEST!",
        };
        ShowCloud(repairedStrings[Random.Range(0, repairedStrings.Count)]);
    }

    public void StartEngine()
    {
        IsEmpty = false;
        ShowCloud("LET'S GO!");
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
        else if (other.gameObject.CompareTag("Crossing"))
        {
            ShowCloud("MAKE A TURN HERE!");
        }    
        else if (other.gameObject.CompareTag("CustomMessage"))
        {
            var customMessage = other.gameObject.GetComponent<CustomMessage>();
            if (customMessage != null && customMessage.Message != "")
            {
                ShowCloud(customMessage.Message.ToUpper(), isSmallFont: customMessage.SmallFont);
            }
        }
        else if (other.gameObject.CompareTag("Victory"))
        {
            if (GameManager.I != null)
            {
                var victory = other.gameObject.GetComponent<VictoryArea>();
                if (victory != null)
                {
                    if (victory.MessageOnReached != null && victory.MessageOnReached.Trim() != "")
                    {
                        ShowCloud(victory.MessageOnReached);
                    }
                    GameManager.I.Victory(victory.LoadNextScene);
                }
                else
                {
                    GameManager.I.Victory();  
                } 
            }
            else
            {
                ShowCloud("WOOHOO!");
            }
        }
    }

    private void UpdateHealth()
    {
        if (health < 0)
            return;

        var decrease = wasBrokenOnce ? HealthDecreasePerSecond : HealthDecreaseInitial;
        health -= Time.deltaTime * decrease;
        if (health <= 0)
        {
            health = 0f;
            GameManager.I.BreakCar();
        }
    }

    public void ShowCloud(string text, float duration = 5f, float delay = 0f, bool isSmallFont = false)
    {
        if (cloudCanvasGroup == null)
            return;
        
        if (cloudAnimation != null)
        {
            StopCoroutine(cloudAnimation);
        }

        cloudText.fontSize = isSmallFont ? (int)(initialFontSize * 0.6f) : initialFontSize;
        cloudAnimation = AnimateCloud(text, duration, delay);
        StartCoroutine(cloudAnimation);
    }

    private IEnumerator AnimateCloud(string text, float duration, float delay = 0f)
    {
        cloudText.text = text;
        cloudCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(delay);
        
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.deltaTime / 0.4f;
            cloudCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, v);
            yield return null;
            
        }
        yield return new WaitForSeconds(duration);
        
        v = 0f;
        while (v <= 1f)
        {
            v += Time.deltaTime / 0.4f;
            cloudCanvasGroup.alpha = Mathf.SmoothStep(1f, 0f, v);
            yield return null;
        }
    }
}

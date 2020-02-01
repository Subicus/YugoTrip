using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrokenPartsManager : MonoBehaviour
{
    #region Properties

    public float MinPower;
    public float MaxPower;

    public GameObject[] BrokenPartsPrefabs;

    #endregion

    #region Fields

    private Driver yugo;
    private List<GameObject> brokenParts = new List<GameObject>();
    private int requiredNumberOfParts;

    #endregion

    #region Initialization

    private void Start()
    {
        yugo = FindObjectOfType<Driver>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateBrokenParts(5, 3);
        }
    }

    #endregion

    #region Public

    public void RepairWithPart(BrokenPart part)
    {
        brokenParts.Remove(part.gameObject);
        Destroy(part.gameObject);
        requiredNumberOfParts--;

        if (requiredNumberOfParts == 0)
        {
            GameManager.I.RepairCar();
        }
    }

    public void GenerateBrokenParts(int numberOfParts, int required)
    {
        requiredNumberOfParts = required;
        RemoveAllParts();
        
        // add new parts
        var yugoPosition = yugo.transform.position;
        for (int i = 0; i < numberOfParts; i++)
        {
            var positionOffset = Random.insideUnitCircle * 3f;
            var position = yugoPosition + new Vector3(positionOffset.x, 2f, positionOffset.y);
            var brokenPart = Instantiate(BrokenPartsPrefabs[i % BrokenPartsPrefabs.Length], 
                position, Random.rotation, transform );
            brokenParts.Add(brokenPart);
            
            // add power
            var rb = brokenPart.GetComponent<Rigidbody>();
            var offset = Random.insideUnitCircle * Random.Range(MinPower, MaxPower);
            rb.AddForce(new Vector3(offset.x, 0f, offset.y));
        }
    }

    public void RemoveAllParts()
    {
        // remove old
        for (int i = 0; i < brokenParts.Count; i++)
        {
            Destroy(brokenParts[i]);
        }
        brokenParts.Clear();
    }

    #endregion
}

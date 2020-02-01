using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Speedometer : MonoBehaviour
{
    #region Properties

    public Transform Needle;

    public CanvasGroup MyCanvasGroup;

    [Range(0,1)]
    public float RotationNormalized;

    [Range(0,1)]
    public float RotationBounceOffset;

    public float RotationBounceTime = 0.1f;

    public float MinRotation;
    public float MaxRotation;

    #endregion

    #region Fields

    private float rotationOffset;

    #endregion

    #region Update

    private void Awake()
    {
        MyCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(DoBounce());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Update()
    {
        Needle.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(MinRotation, MaxRotation, RotationNormalized + rotationOffset));
    }

    #endregion

    #region Private

    private IEnumerator DoBounce()
    {
        while (true)
        {
            rotationOffset = Random.Range(-RotationBounceOffset, RotationBounceOffset);
            yield return new WaitForSecondsRealtime(RotationBounceTime);
        }
    }

    #endregion
}

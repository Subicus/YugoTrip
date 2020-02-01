using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Speedometer : MonoBehaviour
{
    #region Properties

    public Transform Needle;

    public CanvasGroup MyCanvasGroup;

    [Range(0,1)]
    public float RotationNormalized;

    public float MinRotation;
    public float MaxRotation;

    #endregion

    #region Update

    private void Awake()
    {
        MyCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Update()
    {
        Needle.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(MinRotation,MaxRotation,RotationNormalized));
    }

    #endregion

    #region Public

    public void SetAlpha(float a)
    {
        MyCanvasGroup.alpha = a;
    }

    #endregion
}

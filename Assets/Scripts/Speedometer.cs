using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Speedometer : MonoBehaviour
{
    #region Properties

    public Transform Needle;

    [Range(0,1)]
    public float RotationNormalized;

    public float MinRotation;
    public float MaxRotation;

    #endregion

    #region Update

    public void Update()
    {
        Needle.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(MinRotation,MaxRotation,RotationNormalized));
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPiece : MonoBehaviour
{
    public Transform startPivot;
    public Transform endPivot;

    private void OnDrawGizmos()
    {
        if (startPivot)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(startPivot.position, startPivot.forward * 10);
            Gizmos.DrawWireCube(startPivot.position, Vector3.one);
        }
        if (endPivot)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(endPivot.position, endPivot.forward * 10);
            Gizmos.DrawWireCube(endPivot.position, Vector3.one);
        }
    }
}

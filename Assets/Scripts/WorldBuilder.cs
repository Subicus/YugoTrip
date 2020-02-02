using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public LevelData level;
    public int pieceCount = 20;

    void Start()
    {
        Transform prevTransformPivot = transform;
        for (int i = 0; i < pieceCount; i++)
        {
            var piecePrefab = level.roadPieces[Random.Range(0, level.roadPieces.Length)];
            Place(piecePrefab, ref prevTransformPivot);
        }

        Place(level.endPiece, ref prevTransformPivot);
    }

    void Place(GameObject piecePrefab, ref Transform prevTransformPivot)
    {
        GameObject go = Instantiate(piecePrefab);

        var piece = go.GetComponent<RoadPiece>();
        Vector3 offset = piece.transform.position - piece.startPivot.position;
        go.transform.position = prevTransformPivot.position + offset;
        prevTransformPivot = piece.endPivot;
    }
}

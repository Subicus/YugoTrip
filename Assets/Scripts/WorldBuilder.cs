﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public LevelData level;

    void Start()
    {
        int pieceCount = 10;

        Transform prevTransformPivot = transform;
        for (int i = 0; i < pieceCount; i++)
        {
            var piecePrefab = level.roadPieces[Random.Range(0, level.roadPieces.Length)];
            GameObject go = Instantiate(piecePrefab);

            var piece = go.GetComponent<RoadPiece>();
            Vector3 offset = piece.transform.position - piece.startPivot.position;
            go.transform.position = prevTransformPivot.position + offset;
            prevTransformPivot = piece.endPivot;
        }
    }
}

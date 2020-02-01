using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPart : MonoBehaviour
{
    public Color selected1Color;
    public Color selected2Color;
    public Color normalColor;
    
    private Renderer myRenderer;
    private Rigidbody rb;

    private Transform initialParent;

    private enum State
    {
        None,
        Selected,
        Taken,
    }

    private State state;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialParent = transform.parent;
        myRenderer.material.color = normalColor;
    }

    public void SetSelected(bool isSelected, bool isFirstPlayer)
    {
        if (state == State.Taken)
            return;
        
        myRenderer.material.color = isSelected ? (isFirstPlayer ? selected1Color : selected2Color) : normalColor;
        state = isSelected ? State.Selected : State.None;
    }

    public void SetTaken(Transform playerTransform)
    {
        if (playerTransform != null)
        {
            state = State.Taken;
            transform.SetParent(playerTransform);
            transform.position = playerTransform.position + Vector3.up;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            transform.SetParent(initialParent);
            rb.constraints = RigidbodyConstraints.None;
            state = State.None;
        }
    }
}

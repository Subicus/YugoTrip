using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager I { get; private set; }

    public enum GameState
    {
        Driving,
        Repairing,
        Questions,
    }

    private GameState state;
    public GameState State
    {
        get => state;
        set
        {
            state = value;
            StateUpdated();
        }
    }

    public Person firstPlayer;
    public Person secondPlayer;
    public Driver yugo;
    public QuestionGame questionGame;
    public CameraManager cameraManager;
    public BrokenPartsManager partsManager;

    public bool IsDriving => State == GameState.Driving;

    #region Initialization

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        State = GameState.Driving;
    }


    private void Update()
    {
        if (State == GameState.Driving && Input.GetKeyDown(KeyCode.R))
        {
            BreakCar();
        }
    }

    #endregion

    #region Public

    public void BreakCar()
    {
        State = GameState.Repairing;
    }

    public void RepairCar()
    {
        State = GameState.Driving;
    }

    #endregion

    #region Private

    private void StateUpdated()
    {
        switch (State)
        {
            case GameState.Driving:
                firstPlayer.GoInCar();
                secondPlayer.GoInCar();
                yugo.Repair();
                partsManager.RemoveAllParts();
                cameraManager.FollowAdditionalTargets = false;
                break;
            case GameState.Repairing:
                cameraManager.FollowAdditionalTargets = true;
                partsManager.GenerateBrokenParts(5, 3);
                firstPlayer.GoOutOfCar();
                secondPlayer.GoOutOfCar();
                yugo.Break();
                break;
            case GameState.Questions:
                // TODO:
                break;
        }
    }

    #endregion
}

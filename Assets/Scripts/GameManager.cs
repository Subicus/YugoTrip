using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager I { get; private set; }

    public enum GameState
    {
        Driving,
        Repairing,
        Questions,
        Exploded
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

    public float MinTimeForQuestion;
    public float MaxTimeForQuestion;
    public float InitialTimeForQuestion;

    private float timeForQuestion;
    
    public bool IsDriving => State == GameState.Driving;

    #region Initialization

    private void Awake()
    {
        I = this;
        timeForQuestion = InitialTimeForQuestion;
    }

    private void Start()
    {
        State = GameState.Driving;
        firstPlayer.GoInCar();
        secondPlayer.GoInCar();
    }


    private void Update()
    {
        if (State == GameState.Driving)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                BreakCar();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ExplodeCar();
            }

            // check for question
            timeForQuestion -= Time.deltaTime;
            if (timeForQuestion <= 0f)
            {
                State = GameState.Questions;
                questionGame.StartNewGame();
            }
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
    
    public void ExplodeCar()
    {
        State = GameState.Exploded;
    }

    public void FinishQuestion()
    {
        Time.timeScale = 1f;
        State = GameState.Driving;
        timeForQuestion = Random.Range(MinTimeForQuestion, MaxTimeForQuestion);
    }

    #endregion

    #region Private

    private void StateUpdated()
    {
        switch (State)
        {
            case GameState.Driving:
                if (yugo.IsBroken)
                {
                    firstPlayer.GoInCar();
                    secondPlayer.GoInCar();
                    yugo.Repair();
                    partsManager.RemoveAllParts();
                }
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
                Time.timeScale = 0f;
                break;
            case GameState.Exploded:
                yugo.Explode();
                break;
        }
    }

    #endregion
}

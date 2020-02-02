using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager I { get; private set; }

    public enum GameState
    {
        Driving,
        Repairing,
        Questions,
        Exploded,
        Victory,
        Running,
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
    public EndPanel endPanel;

    public float MinTimeForQuestion;
    public float MaxTimeForQuestion;
    public float InitialTimeForQuestion;

    private float timeForQuestion;
    private bool isFirstQuestion = true;
    
    public bool IsDriving => State == GameState.Driving;
    public bool IsRepairing => State == GameState.Repairing;
    public bool IsRunning => State == GameState.Running;

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartRunning();
                return;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                BreakCar();
                return;
            }
            
            // check for question
            timeForQuestion -= Time.deltaTime;
            if (timeForQuestion <= 0f)
            {
                State = GameState.Questions;
                questionGame.StartNewGame(isFirstQuestion ? 5f : 3f);
                isFirstQuestion = false;
            }
        }
    }

    #endregion

    #region Public

    public void BreakCar()
    {
        State = GameState.Repairing;
    }

    public void FixedCar()
    {
        State = GameState.Running;
    }
    
    public void ExplodeCar()
    {
        State = GameState.Exploded;
        endPanel.DoEndAnimation(false);
    }

    public void Victory()
    {
        State = GameState.Victory;
        endPanel.DoEndAnimation(true);
    }

    public void FinishQuestion()
    {
        Time.timeScale = 1f;
        State = GameState.Driving;
        timeForQuestion = Random.Range(MinTimeForQuestion, MaxTimeForQuestion);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void StartRunning()
    {
        firstPlayer.GoOutOfCar();
        secondPlayer.GoOutOfCar();
        yugo.EmptyOut();
        State = GameState.Running;
    }

    public void GoInCar()
    {
        if (firstPlayer.IsInCar && secondPlayer.IsInCar)
        {
            State = GameState.Driving;
            yugo.StartEngine();
        }
    }

    #endregion

    #region Private

    private void StateUpdated()
    {
        switch (State)
        {
            case GameState.Running:
                if (yugo.IsBroken)
                {
                    yugo.Repair();
                    partsManager.RemoveAllParts();
                }
                cameraManager.FollowAdditionalTargets = true;
                break;
            case GameState.Driving:
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

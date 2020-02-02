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

    private int hearts;

    public int Hearts
    {
        get => hearts;
        set
        {
            mainUi.RefreshHearts(hearts, value);
            hearts = value;
        }
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
    public MainUI mainUi;

    public float MinTimeForQuestion;
    public float MaxTimeForQuestion;
    public float InitialTimeForQuestion;

    private float timeForQuestion;
    private bool isFirstQuestion = true;
    private Action endCallback;
    
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
        hearts = 3;
        mainUi.RefreshHearts(hearts,hearts, true);
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
            if (Input.GetKeyDown(KeyCode.T))
            {
                State = GameState.Questions;
                questionGame.StartNewGame(3f);
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
        endPanel.DoEndAnimation(false, ReloadGame, true);
    }

    public void Victory(Action callback = null)
    {
        endCallback = callback;
        State = GameState.Victory;
        endPanel.DoEndAnimation(true, endCallback, true);
    }

    public void QuestionAnswered(bool isRepaired)
    {
        if (isRepaired) Hearts++;
        else Hearts--;
    }

    public void FinishQuestion()
    {
        Time.timeScale = 1f;
        if (Hearts == 0)
        {
            State = GameState.Exploded;
            endPanel.DoEndAnimation(false, ReloadGame);
            return;
        }
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
                yugo.StartCoroutine(StartBreaking());
                break;
            case GameState.Questions:
                Time.timeScale = 0f;
                break;
            case GameState.Exploded:
                yugo.Explode();
                break;
        }
    }

    private IEnumerator StartBreaking()
    {
        yugo.StartBreaking();
        yield return new WaitForSeconds(1f);
        
        yugo.Break();
        cameraManager.FollowAdditionalTargets = true;
        partsManager.GenerateBrokenParts(5, 3);
        firstPlayer.GoOutOfCar();
        secondPlayer.GoOutOfCar();
    }

    #endregion
}

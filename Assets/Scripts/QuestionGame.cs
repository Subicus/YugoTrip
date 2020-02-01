using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionGame : MonoBehaviour
{
    #region Data

    public class QuestionData
    {
        public string Question;
        public string Answer1;
        public string Answer2;
        public string Answer3;
        public string Answer4;
    }

    #endregion
    
    #region Properties

    public Text questionText;
    public Text answer1;
    public Text answer2;
    public Text answer3;
    public Text answer4;
    public Speedometer Speedometer;

    public Transform player1Arrow;
    public Transform player2Arrow;

    private Animator animator;
    private CanvasGroup canvasGroup;
    private CanvasGroup player1ArrowCanvas;
    private CanvasGroup player2ArrowCanvas;

    private bool canInput;
    private float answerDuration;
    private int player1ChoiceIndex;
    private int player2ChoiceIndex;
    
    private static readonly int IntroKey = Animator.StringToHash("intro");
    private static readonly int ResetKey = Animator.StringToHash("reset");
    private static readonly int Answer1Key = Animator.StringToHash("answer1");
    private static readonly int Answer2Key = Animator.StringToHash("answer2");
    private static readonly int Answer3Key = Animator.StringToHash("answer3");
    private static readonly int Answer4Key = Animator.StringToHash("answer4");
    private static readonly int ArrowsKey = Animator.StringToHash("arrows");
    
    private static List<int> AnswersKey = new List<int>
    {
        Answer1Key, Answer2Key, Answer3Key, Answer4Key
    };

    #endregion

    #region Initialization

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();

        player1ArrowCanvas = player1Arrow.GetComponent<CanvasGroup>();
        player2ArrowCanvas = player2Arrow.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        ShowQuestion(new QuestionData
        {
            Question = "HEY, GET ME MY FAVOURITE CIGARS... ",
            Answer1 = "DRINA NO FILTER",
            Answer2 = "MARLBRO",
            Answer3 = "NO SMOKING, PLEASE",
            Answer4 = "RED APPLE CIGARS",
        }, 5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger(ResetKey);
            canvasGroup.alpha = 0f;
            ShowQuestion(new QuestionData
            {
                Question = "HEY, GET ME MY FAVOURITE CIGARS... ",
                Answer1 = "DRINA NO FILTER",
                Answer2 = "MARLBRO",
                Answer3 = "NO SMOKING, PLEASE",
                Answer4 = "RED APPLE CIGARS",
            }, 5f);
        }
        if (canInput)
        {
            if (Input.GetKeyDown(KeyCode.W)) player1ChoiceIndex = 0;
            else if (Input.GetKeyDown(KeyCode.D)) player1ChoiceIndex = 1;
            else if (Input.GetKeyDown(KeyCode.S)) player1ChoiceIndex = 2;
            else if (Input.GetKeyDown(KeyCode.A)) player1ChoiceIndex = 3;
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) player2ChoiceIndex = 0;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) player2ChoiceIndex = 1;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) player2ChoiceIndex = 2;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) player2ChoiceIndex = 3;
        }
    }

    #endregion

    #region Public

    public void ShowQuestion(QuestionData questionData, float duration)
    {
        answerDuration = duration;
        canvasGroup.alpha = 1f;
        questionText.text = questionData.Question;
        answer1.text = questionData.Answer1;
        answer2.text = questionData.Answer2;
        answer3.text = questionData.Answer3;
        answer4.text = questionData.Answer4;
        Speedometer.RotationNormalized = 1f;
        Speedometer.RotationBounceOffset = 0.03f;

        player1ArrowCanvas.alpha = player2ArrowCanvas.alpha = 0f;
        
        animator.SetTrigger(IntroKey);
        canInput = true;
        player1ChoiceIndex = Random.Range(0, 3);
        player2ChoiceIndex = Random.Range(0, 3);
    }

    // called from animation
    public void IntroAnimationFinished()
    {
        Debug.Log("Intro animation");
        StartCoroutine(DropNeedleAnimation());
    }

    public void SetPlayerChoices(int p1, int p2)
    {
        if (p1 == p2)
        {
            animator.SetTrigger(AnswersKey[p1]);
        }
        else
        {
            animator.SetTrigger(AnswersKey[p1]);
            animator.SetTrigger(AnswersKey[p2]);
        }
        player1Arrow.rotation = Quaternion.Euler(0f, 0f, -180f - p1 * 90f);
        player2Arrow.rotation = Quaternion.Euler(0f, 0f, -180f - p2 * 90f);
        animator.SetTrigger(ArrowsKey);
    }

    #endregion

    #region Private

    private IEnumerator DropNeedleAnimation()
    {
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / answerDuration;
            Speedometer.RotationNormalized = Mathf.SmoothStep(1f, 0f, v);
            yield return null;
        }
        Speedometer.RotationBounceOffset = 0f;

        canInput = false;
        Debug.Log("TIME FINISHED!");
        SetPlayerChoices(player1ChoiceIndex, player2ChoiceIndex);
    }

    #endregion
}

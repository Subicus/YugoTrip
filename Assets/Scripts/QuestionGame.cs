using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

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

    private Animator animator;
    private CanvasGroup canvasGroup;
    
    
    private static readonly int IntroKey = Animator.StringToHash("intro");
    private static readonly int ResetKey = Animator.StringToHash("reset");
    private static readonly int Answer1Key = Animator.StringToHash("answer1");
    private static readonly int Answer2Key = Animator.StringToHash("answer2");
    private static readonly int Answer3Key = Animator.StringToHash("answer3");
    private static readonly int Answer4Key = Animator.StringToHash("answer4");

    #endregion

    #region Initialization

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
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
        });
    }

    #endregion

    #region Public

    public void ShowQuestion(QuestionData questionData)
    {
        canvasGroup.alpha = 1f;
        questionText.text = questionData.Question;
        answer1.text = questionData.Answer1;
        answer2.text = questionData.Answer2;
        answer3.text = questionData.Answer3;
        answer4.text = questionData.Answer4;
        Speedometer.RotationNormalized = 1f;
        Speedometer.RotationBounceOffset = 0.03f;
        
        animator.SetTrigger(IntroKey);
    }

    // called from animation
    public void IntroAnimationFinished()
    {
        Debug.Log("Intro animation");
        StartCoroutine(DropNeedleAnimation());
    }

    #endregion

    #region Private

    private IEnumerator DropNeedleAnimation()
    {
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 5f;
            Speedometer.RotationNormalized = Mathf.SmoothStep(1f, 0f, v);
            yield return null;
        }
        Speedometer.RotationBounceOffset = 0f;
        
        Debug.LogError("TIME FINISHED!");
        animator.SetTrigger(Answer2Key);
        animator.SetTrigger(Answer1Key);
    }

    #endregion
}

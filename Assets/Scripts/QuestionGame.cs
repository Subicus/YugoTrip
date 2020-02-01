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
    public Transform player1StaticArrow;
    public Transform player2StaticArrow;
    public AnimationCurve staticArrowAnimation;

    public Image endBackImage;
    public Text endText;
    public Color wonColor;
    public Color lostColor;

    private Animator animator;
    private CanvasGroup canvasGroup;
    private CanvasGroup player1ArrowCanvas;
    private CanvasGroup player2ArrowCanvas;
    private CanvasGroup endCanvas;

    private IEnumerator player1StaticArrowAnimation;
    private IEnumerator player2StaticArrowAnimation;

    private bool canInput;
    private float answerDuration;
    private int player1ChoiceIndex;
    private int player2ChoiceIndex;

    private QuestionLoader questionLoader;
    private bool shouldBeReset;
    
    private static readonly int IntroKey = Animator.StringToHash("intro");
    private static readonly int ResetKey = Animator.StringToHash("reset");
    private static readonly int Answer1Key = Animator.StringToHash("answer1");
    private static readonly int Answer2Key = Animator.StringToHash("answer2");
    private static readonly int Answer3Key = Animator.StringToHash("answer3");
    private static readonly int Answer4Key = Animator.StringToHash("answer4");
    private static readonly int ArrowsKey = Animator.StringToHash("arrows");
    private static readonly int EndKey = Animator.StringToHash("end");
    private static readonly int SizePropertyKey = Shader.PropertyToID("_Size");
    private static readonly int Answer1FadeKey = Animator.StringToHash("answer1fade");
    private static readonly int Answer2FadeKey = Animator.StringToHash("answer2fade");
    private static readonly int Answer3FadeKey = Animator.StringToHash("answer3fade");
    private static readonly int Answer4FadeKey = Animator.StringToHash("answer4fade");
    
    private static List<int> AnswersKey = new List<int>
    {
        Answer1Key, Answer2Key, Answer3Key, Answer4Key
    };
    private static List<int> AnswersFadeKey = new List<int>
    {
        Answer1FadeKey, Answer2FadeKey, Answer3FadeKey, Answer4FadeKey
    };

    private Material blurMaterial;

    #endregion

    #region Initialization

    private void Awake()
    {
        questionLoader = FindObjectOfType<QuestionLoader>();
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
        endCanvas = endBackImage.GetComponent<CanvasGroup>();

        player1ArrowCanvas = player1Arrow.GetComponent<CanvasGroup>();
        player2ArrowCanvas = player2Arrow.GetComponent<CanvasGroup>();

        blurMaterial = GetComponent<Image>().material;
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void StartNewGame()
    {
        if (shouldBeReset)
        {
            animator.SetTrigger(ResetKey);
        }
        shouldBeReset = true;
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        endCanvas.alpha = 0f;
        ShowQuestion(questionLoader.GetRandomQuestion(), 3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartNewGame();
        }
        if (canInput)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                player1ChoiceIndex = 0;
                DoPlayer1Animation();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                player1ChoiceIndex = 1;
                DoPlayer1Animation();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                player1ChoiceIndex = 2;
                DoPlayer1Animation();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                player1ChoiceIndex = 3;
                DoPlayer1Animation();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                player2ChoiceIndex = 0;
                DoPlayer2Animation();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                player2ChoiceIndex = 1;
                DoPlayer2Animation();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                player2ChoiceIndex = 2;
                DoPlayer2Animation();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                player2ChoiceIndex = 3;
                DoPlayer2Animation();
            }
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
        StartCoroutine(DoBlurAnimation(true));
        
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
        // activate selected ones
        if (p1 == p2)
        {
            animator.SetTrigger(AnswersKey[p1]);
        }
        else
        {
            animator.SetTrigger(AnswersKey[p1]);
            animator.SetTrigger(AnswersKey[p2]);
        }
        // fade others
        for (int i = 0; i < 4; i++)
        {
            if (i != p1 && i != p2)
            {
                animator.SetTrigger(AnswersFadeKey[i]);
            }
        }
        player1Arrow.rotation = Quaternion.Euler(0f, 0f, - p1 * 90f);
        player2Arrow.rotation = Quaternion.Euler(0f, 0f, - p2 * 90f);
        animator.SetTrigger(ArrowsKey);

        StartCoroutine(DoEndAnimation(p1 == p2));
    }

    // from animation
    public void FadeOutBlur()
    {
        StartCoroutine(DoBlurAnimation(false));
    }

    #endregion

    #region Private

    private void DoPlayer1Animation()
    {
        if (player1StaticArrowAnimation != null)
        {
            StopCoroutine(player1StaticArrowAnimation);
        }
        player1StaticArrowAnimation = DoStaticArrowAnimation(true);
        StartCoroutine(player1StaticArrowAnimation);
    }
    
    private void DoPlayer2Animation()
    {
        if (player2StaticArrowAnimation != null)
        {
            StopCoroutine(player2StaticArrowAnimation);
        }
        player2StaticArrowAnimation = DoStaticArrowAnimation(false);
        StartCoroutine(player2StaticArrowAnimation);
    }

    private IEnumerator DoStaticArrowAnimation(bool isFirstPlayer)
    {
        var staticArrow = isFirstPlayer ? player1StaticArrow : player2StaticArrow;
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 0.3f;
            staticArrow.localScale = Vector3.one * staticArrowAnimation.Evaluate(v);
            yield return null;
        }
    }

    private IEnumerator DoBlurAnimation(bool isBlurred)
    {
        var startValue = isBlurred ? 0f : 20f;
        var endValue = isBlurred ? 20f : 0f;
        var duration = isBlurred ? 1f : 0.3f;
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / duration;
            blurMaterial.SetFloat(SizePropertyKey, Mathf.SmoothStep(startValue, endValue, v));
            yield return null;
        }

        // finished everything
        if (!isBlurred)
        {
            GameManager.I.FinishQuestion();
        }
    }

    private IEnumerator DropNeedleAnimation()
    {
        animator.ResetTrigger(ResetKey);
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

    private IEnumerator DoEndAnimation(bool isWin)
    {
        yield return new WaitForSecondsRealtime(1f);
        
        endBackImage.color = isWin ? wonColor : lostColor;
        endText.text = isWin ? "RELATIONSHIP REPAIRED!" : "RELATIONSHIP BROKEN!";
        
        animator.SetTrigger(EndKey);
    }

    #endregion
}

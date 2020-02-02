using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    #region Properties

    public Color winColor;
    public Color lostColor;
    public Text endText;
    public CanvasGroup overlayCanvasGroup;

    #endregion
    
    #region Fields

    private CanvasGroup myCanvasGroup;
    private Material blurMaterial;
    private bool isEnding;
    
    private static readonly int SizePropertyKey = Shader.PropertyToID("_Size");

    #endregion

    #region Initialization

    private void Awake()
    {
        myCanvasGroup = GetComponent<CanvasGroup>();
        blurMaterial = GetComponent<Image>().material;
    }

    #endregion

    #region Public

    public void DoEndAnimation(bool isWin, Action endCallback = null, bool isDelay = false)
    {
        if (isEnding)
            return;

        isEnding = true;
        var text = isWin ? "BRAVO!\nRELATIONSHIP REPAIRED!" : "RELATIONSHIP BROKEN!";
        var delay = isDelay ? 3f : 0f;
        StartCoroutine(DoAnimation(text, isWin, delay, endCallback));
    }

    #endregion

    #region Private

    private IEnumerator DoAnimation(string text, bool isWin, float delay = 0f, Action endCallback = null)
    {
        myCanvasGroup.alpha = 0f;
        overlayCanvasGroup.alpha = 0f;
        endText.text = text;
        endText.color = isWin ? winColor : lostColor;
        yield return new WaitForSecondsRealtime(delay);

        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 0.8f;
            myCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, v);
            blurMaterial.SetFloat(SizePropertyKey, Mathf.SmoothStep(0f, 20f, v));
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(3f);
        
        v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 0.8f;
            overlayCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, v);
            yield return null;
        }
        
        // just load next game
        endCallback?.Invoke();
    }

    #endregion
}

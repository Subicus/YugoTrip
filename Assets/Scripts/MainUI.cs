using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Text heartText;
    private RectTransform myRectTransform;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void RefreshHearts(int oldHearts, int newHearts, bool isFast = false)
    {
        if (isFast)
        {
            heartText.text = newHearts.ToString();
            return;
        }
        StartCoroutine(DoRefreshHeartsAnimation(oldHearts, newHearts));
    }

    private IEnumerator DoRefreshHeartsAnimation(int oldHearts, int newHearts)
    {
        heartText.text = oldHearts.ToString();
        var v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 0.2f;
            myRectTransform.localScale = Vector3.one * Mathf.SmoothStep(1f, 1.5f, v);
            yield return null;
        }
        
        heartText.text = newHearts.ToString();
        
        v = 0f;
        while (v <= 1f)
        {
            v += Time.unscaledDeltaTime / 0.2f;
            myRectTransform.localScale = Vector3.one * Mathf.SmoothStep(1.5f, 1f, v);
            yield return null;
        }
    }
}

using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : BaseView
{
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetValue;
    private bool isComplete = false;
    private Action onCompleteAction;
    private int _lastPercent = -1;

    private void Update()
    {
        if (!Mathf.Approximately(loadingSlider.value, targetValue))
        {
            loadingSlider.value = Mathf.MoveTowards(
                loadingSlider.value, targetValue, Time.deltaTime * smoothSpeed);

            int pct = (int)loadingSlider.value;
            if (pct != _lastPercent)                 // 값 변할 때만
            {
                loadingText.SetText("{0}%", pct);    // string 할당 없음
                _lastPercent = pct;
            }
        }
        else if (targetValue >= 100f && !isComplete)
        {
            loadingSlider.value = 100f;
            loadingText.SetText("100%");
            isComplete = true;
            onCompleteAction?.Invoke();
        }
    }

    public void SetOnCompleteAction(Action onComplete)
    {
        onCompleteAction = onComplete;
    }

    public void SetValue(float value)
    {
        targetValue = value;
    }

    public override void Show()
    {
        // 재사용 대비 상태 초기화
        isComplete = false;
        targetValue = 0f;
        loadingSlider.value = 0f;
        _lastPercent = -1;
        loadingText.text = "0%";
        base.Show();
    }
}
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

    private void Update()
    {
        if (!Mathf.Approximately(loadingSlider.value, targetValue))
        {
            loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, targetValue, Time.deltaTime * smoothSpeed);
            loadingText.text = $"{(int) loadingSlider.value}%";
        }
        else if (targetValue >= 100f && !isComplete)
        {
            loadingSlider.value = 100f;
            loadingText.text = "100%";
            isComplete = true;
            onCompleteAction?.Invoke();
        }
    }

    public void SetOnCompleteAction(Action onComplete)
    {
        onCompleteAction = onComplete;
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public override void Show()
    {
        isComplete = false;
        base.Show();
        transform.DOLocalMoveY(-2000, 0);
        transform.DOLocalMoveY(0, 0.3f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetValue(float value)
    {
        targetValue = value;
    }
}
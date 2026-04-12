using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour, ILoadingView
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
            loadingText.text = $"{(int)loadingSlider.value}%";
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

    public void Display()
    {
        isComplete = false;
        gameObject.SetActive(true);
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
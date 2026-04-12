using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadingView : MonoBehaviour, ILoadingView
{
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetValue;

    private void Update()
    {
        if (!Mathf.Approximately(loadingSlider.value, targetValue))
        {
            loadingText.text = $"{(int) targetValue}%";
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void Display()
    {
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
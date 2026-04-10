using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoadingView : BaseView<LoadingView>
{
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float smoothSpeed = 5f;

    protected override string ViewName { get; }

    public void SetValue(float value)
    {
        View?.SetLoading(value);
    }


    private float targetValue;

    private void Update()
    {
        if (!Mathf.Approximately(loadingSlider.value, targetValue))
        {
            loadingText.text = $"{(int) targetValue}%";
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }

    public async Task WaitUntilFull()
    {
        while (!Mathf.Approximately(loadingSlider.value, targetValue))
        {
            await Task.Yield();
        }
    }

    public void SetLoading(float value)
    {
        targetValue = value;
    }
}
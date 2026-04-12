using UnityEngine;
using UnityEngine.UI;

public class OptionView : MonoBehaviour, IOptionView
{
    [SerializeField] private Button backButton;
    public void Dispose()
    {
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
    }
}

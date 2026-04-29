using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour, IPauseView
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    public void Dispose()
    {
    }

    public void Display()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        
        resumeButton.onClick.AddListener(Hide);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}

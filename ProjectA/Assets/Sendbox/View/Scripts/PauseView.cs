using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour, IPauseView
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private Action onClosed;

    public void Initialize(Action onClosed)
    {
        this.onClosed = onClosed;
        resumeButton.onClick.AddListener(Hide);
    }

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
        onClosed?.Invoke();
    }
}

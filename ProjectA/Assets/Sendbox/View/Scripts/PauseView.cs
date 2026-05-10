using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour, IPauseView
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private Action onResume;
    private Action onRestart;
    private Action onQuit;

    public void Initialize(Action onResume, Action onRestart, Action onQuit)
    {
        this.onResume = onResume;
        this.onRestart = onRestart;
        this.onQuit = onQuit;
        
        settingButton.onClick.AddListener(OnClickedSetting);
        resumeButton.onClick.AddListener(OnClickedResume);
        restartButton.onClick.AddListener(OnClickedRestart);
        quitButton.onClick.AddListener(OnClickedQuit);

    }

    public void Dispose()
    {
    }

    public void Display()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void OnClickedRestart()
    {
        onRestart?.Invoke();
        Hide();
    }

    private void OnClickedQuit()
    {
        onQuit?.Invoke();
        Hide();
    }

    private void OnClickedSetting()
    {

    }

    private void OnClickedResume()
    {
        onResume?.Invoke();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}

using System;

public interface IPauseView : IView
{
    public void Initialize(Action onResume, Action onRestart, Action onQuit);
}

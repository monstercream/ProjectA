using System;

public interface IPauseView : IView
{
    public void Initialize(Action onClosed);
}

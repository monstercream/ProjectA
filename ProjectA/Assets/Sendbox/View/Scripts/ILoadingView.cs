
using System;

public interface ILoadingView : IView
{
    void SetOnCompleteAction(Action onComplete); // 추가
    public void SetValue(float value);
}
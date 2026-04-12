
using System;

public interface ISystemPopupView : IView
{
    public void Initialize(string title, string message , Action onConfirm = null);
}
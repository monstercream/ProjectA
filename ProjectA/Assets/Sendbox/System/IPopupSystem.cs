using System;
using UnityEngine;

public interface IPopupSystem : ISystem
{
    public void ShowNotice(string title, string message);
    
    public void ShowPopup(string title, string message, Action onConfirm);
}

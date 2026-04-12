using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemPopupView : MonoBehaviour, ISystemPopupView
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;

    public void Display() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void Initialize(string title, string message, Action onConfirm = null)
    {
        titleText.text = title;
        messageText.text = message;
        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Hide();
        });
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

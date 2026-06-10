using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SystemPopupView : BaseView
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private float slideDistance = 300f;

    private RectTransform rectTransform;
    private Vector2 originPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPosition = rectTransform.anchoredPosition;
    }

    public override void Show()
    {
        base.Show();
        transform.DOLocalMoveY(-2000, 0);
        transform.DOLocalMoveY(0, 0.3f);
        rectTransform.anchoredPosition = originPosition + Vector2.up * slideDistance;
        rectTransform.DOAnchorPos(originPosition, animDuration).SetEase(Ease.OutBack);
    }


    public void Hide()
    {
        rectTransform.DOAnchorPos(originPosition - Vector2.up * slideDistance, animDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

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
        rectTransform.DOKill();
        Destroy(gameObject);
    }
}
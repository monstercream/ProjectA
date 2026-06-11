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
        rectTransform.anchoredPosition = originPosition + Vector2.up * slideDistance;
        rectTransform.DOAnchorPos(originPosition, animDuration).SetEase(Ease.OutBack);
    }

    // override로 변경 — 이제 BaseView 타입으로 호출해도 애니메이션 Hide가 실행됨
    public override void Hide()
    {
        rectTransform.DOAnchorPos(originPosition - Vector2.up * slideDistance, animDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void Initialize(string title, string message, Action onConfirm = null)
    {
        titleText.text = title;
        messageText.text = message;

        // 리스너 중복 등록 방지
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Hide();
        });
    }

    private void OnDestroy()
    {
        rectTransform.DOKill();
    }
}
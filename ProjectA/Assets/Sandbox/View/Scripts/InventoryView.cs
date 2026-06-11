using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : BaseView
{
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public override void Show()
    {
        base.Show();
        transform.DOLocalMoveY(-2000, 0);
        transform.DOLocalMoveY(0, 0.3f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

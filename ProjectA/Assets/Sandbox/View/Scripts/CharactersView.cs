using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharactersView : BaseView
{
    [SerializeField] private Button backButton;

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

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
    }
}

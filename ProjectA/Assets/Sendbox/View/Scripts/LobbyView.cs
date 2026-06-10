using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : BaseView
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button charactersButton;
    [SerializeField] private Button profileButton;

    [SerializeField] private Button battleButton;
    [SerializeField] private Button optionButton;

    private SystemPopupView systemPopupView;
    private InventoryView inventoryView;
    private ShopView shopView;
    private SkillView skillView;
    private CharactersView charactersView;
    private OptionView optionView;
    private StageSelectionView stageSelectionView;

    public override void Show()
    {
        base.Show();
        transform.DOLocalMoveY(-2000, 0);
        transform.DOLocalMoveY(0, 0.3f);
    }

    public void Hide() => gameObject.SetActive(false);

    public async Task Start()
    {
        systemPopupView = ViewManager.Instance.Show<SystemPopupView>();
        inventoryView = ViewManager.Instance.Show<InventoryView>();
        shopView = ViewManager.Instance.Show<ShopView>();
        skillView = ViewManager.Instance.Show<SkillView>();
        charactersView = ViewManager.Instance.Show<CharactersView>();
        optionView = ViewManager.Instance.Show<OptionView>();
        stageSelectionView = ViewManager.Instance.Show<StageSelectionView>();

        inventoryButton.onClick.AddListener(inventoryView.Show);
        shopButton.onClick.AddListener(shopView.Show);
        skillButton.onClick.AddListener(skillView.Show);
        charactersButton.onClick.AddListener(charactersView.Show);
        optionButton.onClick.AddListener(optionView.Show);
        battleButton.onClick.AddListener(() =>
        {
            stageSelectionView.Show();
            Hide();
        });
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

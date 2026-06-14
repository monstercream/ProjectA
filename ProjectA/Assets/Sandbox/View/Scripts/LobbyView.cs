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

    // async Task Start() → void Start()
    // Show가 아니라 Get으로 참조만 가져온다 (시작 시 전부 열리는 문제 해결)
    private void Start()
    {
        systemPopupView    = ViewManager.Instance.Get<SystemPopupView>();
        inventoryView      = ViewManager.Instance.Get<InventoryView>();
        shopView           = ViewManager.Instance.Get<ShopView>();
        skillView          = ViewManager.Instance.Get<SkillView>();
        charactersView     = ViewManager.Instance.Get<CharactersView>();
        optionView         = ViewManager.Instance.Get<OptionView>();
        stageSelectionView = ViewManager.Instance.Get<StageSelectionView>();

        // null 방어 — View가 등록 안 된 경우 명확한 에러 후 중단
        if (inventoryView == null || shopView == null || skillView == null ||
            charactersView == null || optionView == null || stageSelectionView == null)
        {
            Debug.LogError("[LobbyView] 일부 View가 ViewManager에 등록되지 않았습니다.");
            return;
        }

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
}
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour, ILobbyView
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button charactersButton;
    [SerializeField] private Button profileButton;
    
    [SerializeField] private Button battleButton;
    [SerializeField] private Button optionButton;
    
    private ISystemPopupView systemPopupView;
    private IInventoryView inventoryView;
    private IShopView shopView;
    private ISkillView skillView;
    private ICharactersView charactersView;
    private IOptionView optionView;

    public void Display() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public async Task  Start()
    {
        systemPopupView = ViewManager.Get<ISystemPopupView>();
        inventoryView = ViewManager.Get<IInventoryView>();
        shopView = ViewManager.Get<IShopView>();
        skillView = ViewManager.Get<ISkillView>();
        charactersView = ViewManager.Get<ICharactersView>();
        optionView = ViewManager.Get<IOptionView>();
        
        inventoryButton.onClick.AddListener(inventoryView.Display);
        shopButton.onClick.AddListener(shopView.Display);
        skillButton.onClick.AddListener(skillView.Display);
        charactersButton.onClick.AddListener(charactersView.Display);
        optionButton.onClick.AddListener(optionView.Display);
        battleButton.onClick.AddListener(() => {Hide();});
        
        systemPopupView.Initialize("Notice", "Test");
        systemPopupView.Display();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

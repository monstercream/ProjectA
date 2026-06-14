using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using JsonModel;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionView : BaseView
{
    [SerializeField] private StageSelectionListItem stageSelectionListItem;
    [SerializeField] private Transform content;
    [SerializeField] private Button backButton;
    private LobbyView lobbyView;
    private IDataManager dataManager;
    private Dictionary<string, StageModel> stageModels;
    private IAddressableManager addressableManager;
    private IngameView ingameView;

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

    public async Task Start()
    {
        Debug.Log("StageSelectionView Start");
        lobbyView = ViewManager.Instance.Get<LobbyView>();
        ingameView = ViewManager.Instance.Get<IngameView>();
        dataManager = SystemsManager.Get<IDataManager>();
        addressableManager = SystemsManager.Get<IAddressableManager>();
        stageModels = dataManager.GetDataDictionary<StageModel>("stage");

        backButton.onClick.AddListener(() =>
        {
            lobbyView.Show();
            Hide();
        });

        CreateStageSelectionListItem(stageModels);
    }

    private void CreateStageSelectionListItem(Dictionary<string, StageModel> models)
    {
        foreach (var model in models)
        {
            var item = Instantiate(stageSelectionListItem, content);
            item.Display(addressableManager, model.Value, OnClickedListItem);
        }
    }

    private async void OnClickedListItem(StageModel model)
    {
        await ingameView.Initialize(model);
        ingameView.Show();
        Hide();
    }
}

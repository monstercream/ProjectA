using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionView : MonoBehaviour, IStageSelectionView
{
    [SerializeField] private StageSelectionListItem stageSelectionListItem;
    [SerializeField] private Transform content;
    [SerializeField] private Button backButton;
    private ILobbyView lobbyView;
    private IDataManager dataManager;
    private Dictionary<string, StageModel> stageModels;
    private IAddressableManager addressableManager;

    public void Dispose()
    {

    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public async Task Start()
    {
        Debug.Log("StageSelectionView Start");
        lobbyView = ViewManager.Get<ILobbyView>();
        dataManager = SystemsManager.Get<IDataManager>();
        addressableManager = SystemsManager.Get<IAddressableManager>();
        stageModels = dataManager.GetDataDictionary<StageModel>("stage");

        backButton.onClick.AddListener(() =>
        {
            lobbyView.Display();
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
        GameObject stage = await addressableManager.InstantiateAsync(model.PrefabPath);
        GameObject car = await addressableManager.InstantiateAsync("Car", model.StartPosition.ToVector3(),
            Quaternion.Euler(model.StartRotation.ToVector3()));
        GameObject miniMap = await addressableManager.InstantiateAsync("MiniMap");
        miniMap.GetComponent<MinimapSystem>().SetTarget(car.transform);
        Hide();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonModel;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Networking;

public class InitController : MonoBehaviour
{
    [SerializeField] private TextAsset[] jsonDatas;
    [SerializeField] private string[] jsonURL;
    private INetworkSystem networkSystem;
    private IDataManager dataManager;

    private async Task<string> LoadJsonFromURL(string url)
    {
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error: {request.error}");

        return request.downloadHandler.text;
    }

    private async void Awake()
    {
        await SystemsManager.Initialize();
        networkSystem = SystemsManager.Get<INetworkSystem>();
        dataManager = SystemsManager.Get<IDataManager>();
        jsonDatas.ForEach(json => { dataManager.SetData(json.name, json.text); });

        await networkSystem.Login("7789B", "42779113-0012-58F2-939B-0870AFAE582E");
        await networkSystem.ExecuteScript("Test");
        await networkSystem.TitleData();
        await networkSystem.UserData();
        await networkSystem.Inventory();
        
        Dictionary<string, CharacterModel> dic = dataManager.GetDataDictionary<CharacterModel>("character");
        dic.ForEach(c => { Debug.LogWarning(c.Value.Name); });
        CharacterModel model = dataManager.GetData<CharacterModel>("character", "character_1");
        Debug.LogWarning(model.Name);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "GetData"))
        {
            Debug.LogWarning("DATA");
        }
    }
}
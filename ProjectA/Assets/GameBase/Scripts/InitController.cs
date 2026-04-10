using System;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Networking;

public class InitController : MonoBehaviour
{
    [SerializeField] private TextAsset[] jsonDatas;
    [SerializeField] private string[] jsonURL;
    private INetworkSystem networkSystem;
    private IDataManager dataManager;
    private ILoadingSystem loading;

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
        loading = SystemsManager.Get<ILoadingSystem>();
        jsonDatas.ForEach(json => { dataManager.SetData(json.name, json.text); });
        loading.Show();

        var tasks = new (Func<Task> action, string label)[]
        {
            (() => networkSystem.Login("7789B", "42779113-0012-58F2-939B-0870AFAE582E"), "로그인 중..."),
            (() => networkSystem.ExecuteScript("Test"), "스크립트 실행 중..."),
            (() => networkSystem.TitleData(), "타이틀 데이터 로드 중..."),
            (() => networkSystem.UserData(), "유저 데이터 로드 중..."),
            (() => networkSystem.Inventory(), "인벤토리 로드 중..."),
        };

        for (int i = 0; i < tasks.Length; i++)
        {
            Debug.Log(tasks[i].label);
            await tasks[i].action();
            loading.SetValue((float)(i + 1) / tasks.Length * 100f);
        }
    }
}
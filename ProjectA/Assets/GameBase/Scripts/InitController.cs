using System;
using System.Linq;
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
    private ILobbyView lobbyView;
    private ILoadingView loadingView;

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
        await ViewManager.Initialize();
        networkSystem = SystemsManager.Get<INetworkSystem>();
        dataManager = SystemsManager.Get<IDataManager>();
        loadingView = ViewManager.Get<ILoadingView>();
        lobbyView = ViewManager.Get<ILobbyView>();
        jsonDatas.ForEach(json => { dataManager.SetData(json.name, json.text); });

        // 로딩 완료 시 실행할 액션 등록
        loadingView.SetOnCompleteAction(() =>
        {
            lobbyView.Display();
            loadingView.Hide();
        });

        var tasks = new (Func<Task> action, string label, bool countProgress)[]
        {
            (() => { loadingView.Display(); return Task.CompletedTask; }, "로딩 시작...", false),
            (() => networkSystem.Login("7789B", "42779113-0012-58F2-939B-0870AFAE582E"), "로그인 중...", true),
            (() => networkSystem.ExecuteScript("Test"), "스크립트 실행 중...", true),
            (() => networkSystem.TitleData(), "타이틀 데이터 로드 중...", true),
            (() => networkSystem.UserData(), "유저 데이터 로드 중...", true),
            (() => networkSystem.Inventory(), "인벤토리 로드 중...", true),
        };

        int totalCount = tasks.Count(t => t.countProgress);
        int doneCount = 0;

        foreach (var task in tasks)
        {
            Debug.Log(task.label);
            await task.action();

            if (task.countProgress)
            {
                doneCount++;
                loadingView.SetValue((float)doneCount / totalCount * 100f);
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InitController : MonoBehaviour
{
    [SerializeField] private TextAsset[] jsonDatas;
    [SerializeField] private string[] jsonURL;
    
    private string GetCatalogURL()
    {
        string baseURL = "https://pub-33d511a0e8b74d2e855de0befcdd341f.r2.dev";

#if UNITY_EDITOR
        // 에디터는 실행 중인 OS 기준으로 플랫폼 결정
#if UNITY_EDITOR_WIN
        const string platform = "StandaloneWindows64";
#elif UNITY_EDITOR_OSX
        const string platform = "StandaloneOSX";
#else
        const string platform = "StandaloneLinux64";
#endif
#elif UNITY_ANDROID
    const string platform = "Android";
#elif UNITY_IOS
    const string platform = "iOS";
#elif UNITY_WEBGL
    const string platform = "WebGL";
#elif UNITY_STANDALONE_OSX
    const string platform = "StandaloneOSX";
#else
    const string platform = "StandaloneWindows64";
#endif

        return $"{baseURL}/{platform}/catalog.bin";
    }

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

    private async Task InitializeAddressables()
    {
        try
        {
            await Addressables.InitializeAsync().Task;
            Debug.Log("Addressables 초기화 완료");
        }
        catch (Exception e)
        {
            throw new Exception($"Addressables 초기화 실패: {e.Message}");
        }
    }

// LoadRemoteCatalog도 동일한 패턴으로 수정
    private async Task LoadRemoteCatalog()
    {
        if (string.IsNullOrEmpty(GetCatalogURL()))
        {
            Debug.LogWarning("remoteCatalogURL이 비어있습니다.");
            return;
        }

        try
        {
            var handle = Addressables.LoadContentCatalogAsync(GetCatalogURL(), false);
            await handle.Task;

            // handle이 release되기 전에 Status 체크
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var ex = handle.OperationException;
                Addressables.Release(handle);
                throw new Exception($"원격 카탈로그 로드 실패: {ex}");
            }

            Addressables.Release(handle);
            Debug.Log($"원격 카탈로그 로드 완료: {GetCatalogURL()}");
        }
        catch (Exception e)
        {
            throw new Exception($"원격 카탈로그 로드 실패: {e.Message}");
        }
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

        loadingView.SetOnCompleteAction(() =>
        {
            lobbyView.Display();
            loadingView.Hide();
        });

        var tasks = new (Func<Task> action, string label, bool countProgress)[]
        {
            (() =>
            {
                loadingView.Display();
                return Task.CompletedTask;
            }, "로딩 시작...", false),
            (() => InitializeAddressables(), "에셋 시스템 초기화 중...", true),
            (() => LoadRemoteCatalog(), "에셋 카탈로그 로드 중...", true),
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
                loadingView.SetValue((float) doneCount / totalCount * 100f);
            }
        }
    }
}
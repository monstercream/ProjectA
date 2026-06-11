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

    private INetworkSystem networkSystem;
    private IDataManager dataManager;
    private LobbyView lobbyView;
    private LoadingView loadingView;

    // ──────────────────────────────────────────────
    // 생명주기
    // ──────────────────────────────────────────────

    private void Awake()
    {
        // 동기 가능한 초기화만 Awake에서 처리
        // ViewManager 등 다른 오브젝트의 Awake가 아직 실행 중일 수 있으므로
        // 시스템 등록만 처리하고 View 접근은 하지 않음
        SystemsManager.Initialize().GetAwaiter().GetResult();
        networkSystem = SystemsManager.Get<INetworkSystem>();
        dataManager   = SystemsManager.Get<IDataManager>();
        jsonDatas.ForEach(json => dataManager.SetData(json.name, json.text));
    }

    private void Start()
    {
        // 모든 오브젝트의 Awake()가 끝난 뒤 실행되므로
        // ViewManager.Instance가 보장된 시점
        _ = InitializeAsync();
    }

    // ──────────────────────────────────────────────
    // 메인 초기화 플로우
    // ──────────────────────────────────────────────

    private async Task InitializeAsync()
    {
        try
        {
            // Instance 대신 GetOrCreate 사용 — 씬에 없으면 자동 생성됨
            var viewManager = ViewManager.GetOrCreate();

            loadingView = viewManager.Show<LoadingView>();
            lobbyView = viewManager.Get<LobbyView>();

            // null 방어: View가 씬에 없으면 명확한 에러를 내고 중단
            if (loadingView == null || lobbyView == null)
            {
                Debug.LogError("[InitController] LoadingView 또는 LobbyView가 씬에 존재하지 않습니다.");
                return;
            }

            loadingView.SetOnCompleteAction(() =>
            {
                lobbyView.Show();
                loadingView.Hide();
            });

            loadingView.Show();

            await RunStepsAsync(new InitStep[]
            {
                new("에셋 시스템 초기화 중...", () => InitializeAddressables()),
                new("에셋 카탈로그 로드 중...", () => LoadRemoteCatalog()),
                new("에셋 카탈로그 업데이트 중...", () => UpdateCatalog()),
                new("로그인 중...", () => networkSystem.Login("7789B", "42779113-0012-58F2-939B-0870AFAE582E")),
                new("스크립트 실행 중...", () => networkSystem.ExecuteScript("Test")),
                new("타이틀 데이터 로드 중...", () => networkSystem.TitleData()),
                new("유저 데이터 로드 중...", () => networkSystem.UserData()),
                new("인벤토리 로드 중...", () => networkSystem.Inventory()),
            });
        }
        catch (Exception e)
        {
            // e.Message만 찍으면 발생 위치를 알 수 없음
            // LogException은 전체 스택 트레이스를 출력해서
            // NRE가 발생한 정확한 파일/줄 번호를 보여줌
            Debug.LogException(e);
        }
    }

    // ──────────────────────────────────────────────
    // 스텝 실행 (진행률 자동 계산)
    // ──────────────────────────────────────────────

    private async Task RunStepsAsync(InitStep[] steps)
    {
        for (int i = 0; i < steps.Length; i++)
        {
            var step = steps[i];
            Debug.Log($"[Init] {step.Label}");

            //loadingView.SetLabel(step.Label);

            await step.Action();

            float progress = (float)(i + 1) / steps.Length * 100f;
            loadingView.SetValue(progress);
        }
    }

    // ──────────────────────────────────────────────
    // Addressables
    // ──────────────────────────────────────────────

    private async Task InitializeAddressables()
    {
        try
        {
            await Addressables.InitializeAsync().Task;
            Debug.Log("[Init] Addressables 초기화 완료");
        }
        catch (Exception e)
        {
            throw new Exception($"Addressables 초기화 실패: {e.Message}");
        }
    }

    private async Task LoadRemoteCatalog()
    {
        string url = GetCatalogURL();

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[Init] remoteCatalogURL이 비어있습니다.");
            return;
        }

        try
        {
            var handle = Addressables.LoadContentCatalogAsync(url, false);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var ex = handle.OperationException;
                Addressables.Release(handle);
                throw new Exception($"원격 카탈로그 로드 실패: {ex}");
            }

            Addressables.Release(handle);
            Debug.Log($"[Init] 원격 카탈로그 로드 완료: {url}");
        }
        catch (Exception e)
        {
            throw new Exception($"원격 카탈로그 로드 실패: {e.Message}");
        }
    }

    private async Task UpdateCatalog()
    {
        try
        {
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            await checkHandle.Task;

            if (checkHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(checkHandle);
                Debug.LogWarning("[Init] 카탈로그 업데이트 확인 실패 - 기존 카탈로그로 진행합니다.");
                return;
            }

            var catalogsToUpdate = checkHandle.Result;
            Addressables.Release(checkHandle);

            if (catalogsToUpdate == null || catalogsToUpdate.Count == 0)
            {
                Debug.Log("[Init] 업데이트할 카탈로그 없음");
                return;
            }

            Debug.Log($"[Init] 카탈로그 업데이트 대상: {catalogsToUpdate.Count}개");

            var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);
            await updateHandle.Task;

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                var ex = updateHandle.OperationException;
                Addressables.Release(updateHandle);
                throw new Exception($"카탈로그 업데이트 실패: {ex}");
            }

            Addressables.Release(updateHandle);
            Debug.Log("[Init] 카탈로그 업데이트 완료");
        }
        catch (Exception e)
        {
            throw new Exception($"카탈로그 업데이트 중 오류: {e.Message}");
        }
    }

    // ──────────────────────────────────────────────
    // URL 헬퍼
    // ──────────────────────────────────────────────

    private string GetCatalogURL()
    {
        const string baseURL = "https://pub-33d511a0e8b74d2e855de0befcdd341f.r2.dev";

#if UNITY_EDITOR
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

    private async Task<string> LoadJsonFromURL(string url)
    {
        using var request = UnityWebRequest.Get(url);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"[Init] JSON 로드 실패: {request.error}");

        return request.downloadHandler.text;
    }

    // ──────────────────────────────────────────────
    // 내부 타입
    // ──────────────────────────────────────────────

    private readonly struct InitStep
    {
        public string Label      { get; }
        public Func<Task> Action { get; }

        public InitStep(string label, Func<Task> action)
        {
            Label  = label;
            Action = action;
        }
    }
}
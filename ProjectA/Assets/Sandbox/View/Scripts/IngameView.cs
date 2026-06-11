using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameView : BaseView
{
    [Header("Hold Buttons (누르고 있는 동안 입력)")]
    [SerializeField] private TouchButton AccelateButton;
    [SerializeField] private TouchButton BrakeButton;
    [SerializeField] private TouchButton SteerLeftButton;
    [SerializeField] private TouchButton SteerRightButton;

    [Header("Click Buttons (한 번 누름)")]
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button CameraButton;

    // ──────────────────────────────────────────────
    // 입력 상태 (다른 컴포넌트가 매 프레임 읽어감)
    // ──────────────────────────────────────────────

    /// <summary>-1(좌) ~ +1(우). 양쪽 동시 누름은 0.</summary>
    public float Horizontal { get; private set; }

    /// <summary>가속 입력 중인지 (터치 or 키보드 ↑/W).</summary>
    public bool IsAccelerating { get; private set; }

    /// <summary>브레이크 입력 중인지 (터치 or 키보드 ↓/S).</summary>
    public bool IsBraking { get; private set; }

    public bool IsLeftHeld { get; private set; }
    public bool IsRightHeld { get; private set; }

    // ──────────────────────────────────────────────
    // 의존성 & 상태
    // ──────────────────────────────────────────────

    private ICameraSystem cameraSystem;
    private IAddressableManager addressableManager;
    private Transform carTransform;

    private readonly (float distance, float height, float posSpeed, float rotSpeed)[] cameraPresets =
    {
        (distance: 5f, height: 1f, posSpeed: 20f, rotSpeed: 20f),
        (distance: 10f, height: 3f, posSpeed: 10f, rotSpeed: 10f),
        (distance: 7f, height: 5f, posSpeed: 15f, rotSpeed: 15f),
        (distance: 3f, height: 1f, posSpeed: 25f, rotSpeed: 25f),
    };

    private int cameraPresetIndex = 0;

    private PauseView pauseView;
    private LobbyView lobbyView;
    private GameObject stage;
    private GameObject car;
    private GameObject miniMap;
    private StageModel stageModel;

    private Keyboard keyboard;

    // ──────────────────────────────────────────────
    // 생명주기
    // ──────────────────────────────────────────────

    public async Task Initialize(StageModel model)
    {
        addressableManager = SystemsManager.Get<IAddressableManager>();
        cameraSystem       = SystemsManager.Get<ICameraSystem>();
        pauseView          = ViewManager.Instance.Get<PauseView>();
        lobbyView          = ViewManager.Instance.Get<LobbyView>();
        keyboard           = Keyboard.current;

        stageModel = model;
        pauseView.Initialize(Resume, Restart, Quit);

        RegisterClickEvents();
        await RaceSetting(model);
    }

    private void Update()
    {
        // 활성 상태일 때만 입력 처리 (Pause 중엔 동작 안 함)
        if (!IsVisible) return;

        UpdateInput();
    }

    // ──────────────────────────────────────────────
    // 입력 통합 — 키보드 + 터치
    // ──────────────────────────────────────────────

    private void UpdateInput()
    {
        // 키보드 입력
        bool keyLeft = keyboard != null &&
                       (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed);
        bool keyRight = keyboard != null &&
                        (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed);
        bool keyAccel = keyboard != null &&
                        (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed);
        bool keyBrake = keyboard != null &&
                        (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed);

        // 터치 입력
        bool touchLeft  = SteerLeftButton  != null && SteerLeftButton.IsPressed;
        bool touchRight = SteerRightButton != null && SteerRightButton.IsPressed;
        bool touchAccel = AccelateButton   != null && AccelateButton.IsPressed;
        bool touchBrake = BrakeButton      != null && BrakeButton.IsPressed;

        // 통합
        IsLeftHeld     = keyLeft  || touchLeft;
        IsRightHeld    = keyRight || touchRight;
        IsAccelerating = keyAccel || touchAccel;
        IsBraking      = keyBrake || touchBrake;

        // Horizontal 계산
        float h = 0f;
        if (IsLeftHeld)  h -= 1f;
        if (IsRightHeld) h += 1f;
        Horizontal = h;
    }

    // ──────────────────────────────────────────────
    // 스테이지 로드 / 해제
    // ──────────────────────────────────────────────

    private async Task RaceSetting(StageModel model)
    {
        Vector3 startPos    = model.StartPosition.ToVector3();
        Quaternion startRot = Quaternion.Euler(model.StartRotation.ToVector3());

        stage   = await addressableManager.InstantiateAsync(model.PrefabPath);
        car     = await addressableManager.InstantiateAsync("Car", startPos, startRot);
        miniMap = await addressableManager.InstantiateAsync("MiniMap");

        carTransform = car.transform;
        cameraSystem.ChaseTarget(carTransform, 5, 1, 20, 20);
        miniMap.GetComponent<MinimapSystem>().SetTarget(carTransform);
    }

    private async Task RaceFinished()
    {
        await addressableManager.ReleaseInstanceAsync(stage);
        await addressableManager.ReleaseInstanceAsync(car);
        await addressableManager.ReleaseInstanceAsync(miniMap);

        stage = null;
        car = null;
        miniMap = null;
    }

    // ──────────────────────────────────────────────
    // 클릭 버튼 (Pause / Camera)
    // ──────────────────────────────────────────────

    private void RegisterClickEvents()
    {
        PauseButton.onClick.AddListener(OnPauseButtonClick);
        CameraButton.onClick.AddListener(OnCameraButtonClick);
    }

    public void OnPauseButtonClick()
    {
        Hide();
        pauseView.Show();
    }

    public void OnCameraButtonClick()
    {
        cameraPresetIndex = (cameraPresetIndex + 1) % cameraPresets.Length;

        var preset = cameraPresets[cameraPresetIndex];
        cameraSystem.ChaseTarget(
            carTransform,
            preset.distance,
            preset.height,
            preset.posSpeed,
            preset.rotSpeed);
    }

    // ──────────────────────────────────────────────
    // Pause 콜백
    // ──────────────────────────────────────────────

    private void Resume()
    {
        pauseView.Hide();
        Show();
    }

    private async void Restart()
    {
        await RaceFinished();
        await RaceSetting(stageModel);
        Resume();
    }

    private async void Quit()
    {
        await RaceFinished();
        pauseView.Hide();
        lobbyView.Show();
    }

    // ──────────────────────────────────────────────
    // 정리
    // ──────────────────────────────────────────────

    protected void OnDestroy()
    {
        PauseButton?.onClick.RemoveAllListeners();
        CameraButton?.onClick.RemoveAllListeners();
    }
}
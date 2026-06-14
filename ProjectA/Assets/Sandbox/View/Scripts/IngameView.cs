using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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

    [Header("Virtual Keyboard Keys (터치 시 시뮬레이션할 키)")]
    [SerializeField] private Key accelerateKey = Key.UpArrow;
    [SerializeField] private Key brakeKey      = Key.DownArrow;
    [SerializeField] private Key steerLeftKey  = Key.LeftArrow;
    [SerializeField] private Key steerRightKey = Key.RightArrow;

    // ──────────────────────────────────────────────
    // 의존성 & 상태
    // ──────────────────────────────────────────────

    private ICameraSystem      cameraSystem;
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

    // 이전 프레임의 터치 상태 — 변경된 순간만 가상 키 이벤트 발행
    private bool prevAccel, prevBrake, prevLeft, prevRight;

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

        if (keyboard == null)
            Debug.LogError("[IngameView] Keyboard.current가 null입니다. Input System 설정을 확인하세요.");

        stageModel = model;
        pauseView.Initialize(Resume, Restart, Quit);

        RegisterClickEvents();
        await RaceSetting(model);
    }

    private void Update()
    {
        if (!IsVisible) return;

        // 터치 버튼 상태를 가상 키보드 입력으로 변환
        SyncTouchToKeyboard();
    }

    // ──────────────────────────────────────────────
    // 가상 키보드 — 터치 버튼 → 키보드 키 이벤트 주입
    // 이렇게 하면 기존 InputAction 바인딩(키보드 ←/→ 등)이 그대로 동작
    // ──────────────────────────────────────────────

    private void SyncTouchToKeyboard()
    {
        if (keyboard == null) return;

        UpdateKey(SteerLeftButton,  steerLeftKey,  ref prevLeft);
        UpdateKey(SteerRightButton, steerRightKey, ref prevRight);
        UpdateKey(AccelateButton,   accelerateKey, ref prevAccel);
        UpdateKey(BrakeButton,      brakeKey,      ref prevBrake);
    }

    private void UpdateKey(TouchButton button, Key key, ref bool prevState)
    {
        if (button == null) return;

        bool current = button.IsPressed;

        // 상태 변경된 순간에만 이벤트 발행 (down/up)
        if (current != prevState)
        {
            SendKeyEvent(key, current);
            prevState = current;
        }
    }

    private void SendKeyEvent(Key key, bool isPressed)
    {
        using (StateEvent.From(keyboard, out var eventPtr))
        {
            var control = keyboard[key];
            control.WriteValueIntoEvent(isPressed ? 1f : 0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }

    /// <summary>현재 눌려있는 가상 키 모두 강제 해제 (Pause/Quit 시 안전장치).</summary>
    private void ReleaseAllVirtualKeys()
    {
        if (keyboard == null) return;

        if (prevLeft)  { SendKeyEvent(steerLeftKey,  false); prevLeft  = false; }
        if (prevRight) { SendKeyEvent(steerRightKey, false); prevRight = false; }
        if (prevAccel) { SendKeyEvent(accelerateKey, false); prevAccel = false; }
        if (prevBrake) { SendKeyEvent(brakeKey,      false); prevBrake = false; }
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
        ReleaseAllVirtualKeys();  // 일시정지 시 입력 끊기
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
        ReleaseAllVirtualKeys();
        await RaceFinished();
        await RaceSetting(stageModel);
        Resume();
    }

    private async void Quit()
    {
        ReleaseAllVirtualKeys();
        await RaceFinished();
        pauseView.Hide();
        lobbyView.Show();
    }

    // ──────────────────────────────────────────────
    // 정리
    // ──────────────────────────────────────────────

    public override void Hide()
    {
        ReleaseAllVirtualKeys();  // 화면 숨길 때 가상 키 해제
        base.Hide();
    }

    protected void OnDestroy()
    {
        ReleaseAllVirtualKeys();
        PauseButton?.onClick.RemoveAllListeners();
        CameraButton?.onClick.RemoveAllListeners();
    }
}
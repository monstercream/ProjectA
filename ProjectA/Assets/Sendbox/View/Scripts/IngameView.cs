using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameView : MonoBehaviour, IIngameView
{
    [SerializeField] private Button AccelateButton;
    [SerializeField] private Button BrakeButton;
    [SerializeField] private Button SteerLeftButton;
    [SerializeField] private Button SteerRightButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button CameraButton;

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

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction steerLeftAction;
    private InputAction steerRightAction;
    private IPauseView pauseView;
    private ILobbyView lobbyView;
    private GameObject stage;
    private GameObject car;
    private GameObject miniMap;
    private StageModel stageModel;

    public async Task Initialize(StageModel model)
    {
        addressableManager = SystemsManager.Get<IAddressableManager>();
        cameraSystem = SystemsManager.Get<ICameraSystem>();
        pauseView = ViewManager.Get<IPauseView>();
        lobbyView = ViewManager.Get<ILobbyView>();
        stageModel = model;

        RegisterButtonEvents();
        await RaceSetting(model);
    }

    private async Task RaceSetting(StageModel model)
    {
        Vector3 startPos = model.StartPosition.ToVector3();
        Quaternion startRot = Quaternion.Euler(model.StartRotation.ToVector3());

        stage = await addressableManager.InstantiateAsync(model.PrefabPath);
        car = await addressableManager.InstantiateAsync("Car", startPos, startRot);
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

    private void RegisterButtonEvents()
    {
        AccelateButton.onClick.AddListener(OnAccelateButtonClick);
        BrakeButton.onClick.AddListener(OnBrakeButtonClick);
        SteerLeftButton.onClick.AddListener(OnSteerLeftButtonClick);
        SteerRightButton.onClick.AddListener(OnSteerRightButtonClick);
        PauseButton.onClick.AddListener(OnPauseButtonClick);
        CameraButton.onClick.AddListener(OnCameraButtonClick);
    }

    public void OnAccelateButtonClick()
    {
        // 차량 전진 처리
    }

    public void OnBrakeButtonClick()
    {
        // 차량 브레이크 처리
    }

    public void OnSteerLeftButtonClick()
    {
        // 차량 좌회전 처리
    }

    public void OnSteerRightButtonClick()
    {
        // 차량 우회전 처리
    }

    public void OnPauseButtonClick()
    {
        Hide();
        pauseView.Initialize(Resume, Restart, Quit);
        pauseView.Display();
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

    public void Dispose()
    {
        accelerateAction?.Disable();
        brakeAction?.Disable();
        steerLeftAction?.Disable();
        steerRightAction?.Disable();

        accelerateAction?.Dispose();
        brakeAction?.Dispose();
        steerLeftAction?.Dispose();
        steerRightAction?.Dispose();

        AccelateButton.onClick.RemoveAllListeners();
        BrakeButton.onClick.RemoveAllListeners();
        SteerLeftButton.onClick.RemoveAllListeners();
        SteerRightButton.onClick.RemoveAllListeners();
        PauseButton.onClick.RemoveAllListeners();
        CameraButton.onClick.RemoveAllListeners();
    }

    private void Resume()
    {
        Display();
    }

    private async void Restart()
    {
        await RaceFinished();
        await RaceSetting(stageModel);
        Display();
    }

    private async void Quit()
    {
        await RaceFinished();
        lobbyView.Display();
    }

    public void Display() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
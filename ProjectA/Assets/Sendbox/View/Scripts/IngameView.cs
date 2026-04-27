using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private ICameraSystem       cameraSystem;
    private IAddressableManager addressableManager;
    private Transform           carTransform;

    private readonly (float distance, float height, float posSpeed, float rotSpeed)[] cameraPresets =
    {
        (distance:  5f, height: 1f, posSpeed: 20f, rotSpeed: 20f),
        (distance: 10f, height: 3f, posSpeed: 10f, rotSpeed: 10f),
        (distance:  7f, height: 5f, posSpeed: 15f, rotSpeed: 15f),
        (distance:  3f, height: 1f, posSpeed: 25f, rotSpeed: 25f),
    };
    private int cameraPresetIndex = 0;

    private InputAction accelerateAction;
    private InputAction brakeAction;
    private InputAction steerLeftAction;
    private InputAction steerRightAction;

    public async Task Initialize(StageModel model)
    {
        addressableManager = SystemsManager.Get<IAddressableManager>();
        cameraSystem       = SystemsManager.Get<ICameraSystem>();

        Vector3    startPos = model.StartPosition.ToVector3();
        Quaternion startRot = Quaternion.Euler(model.StartRotation.ToVector3());

        GameObject stage   = await addressableManager.InstantiateAsync(model.PrefabPath);
        GameObject car     = await addressableManager.InstantiateAsync("Car", startPos, startRot);
        GameObject miniMap = await addressableManager.InstantiateAsync("MiniMap");

        carTransform = car.transform;

        SetupInputActions();
        RegisterButtonEvents();

        cameraSystem.ChaseTarget(carTransform, 5, 1, 20, 20);
        miniMap.GetComponent<MinimapSystem>().SetTarget(carTransform);
    }

    private void SetupInputActions()
    {
        accelerateAction = new InputAction(binding: "<Keyboard>/upArrow");
        brakeAction      = new InputAction(binding: "<Keyboard>/space");
        steerLeftAction  = new InputAction(binding: "<Keyboard>/leftArrow");
        steerRightAction = new InputAction(binding: "<Keyboard>/rightArrow");

        // 키 누름 → 버튼을 프로그래밍적으로 Press
        accelerateAction.performed  += ctx => SimulateButtonDown(AccelateButton);
        brakeAction.performed       += ctx => SimulateButtonDown(BrakeButton);
        steerLeftAction.performed   += ctx => SimulateButtonDown(SteerLeftButton);
        steerRightAction.performed  += ctx => SimulateButtonDown(SteerRightButton);

        // 키 뗌 → 버튼 Release
        accelerateAction.canceled   += ctx => SimulateButtonUp(AccelateButton);
        brakeAction.canceled        += ctx => SimulateButtonUp(BrakeButton);
        steerLeftAction.canceled    += ctx => SimulateButtonUp(SteerLeftButton);
        steerRightAction.canceled   += ctx => SimulateButtonUp(SteerRightButton);

        accelerateAction.Enable();
        brakeAction.Enable();
        steerLeftAction.Enable();
        steerRightAction.Enable();
    }

    // 버튼을 손가락으로 누른 것처럼 처리
    private void SimulateButtonDown(Button button)
    {
        ExecuteEvents.Execute(
            button.gameObject,
            new PointerEventData(EventSystem.current),
            ExecuteEvents.pointerDownHandler);
    }

    // 버튼에서 손가락을 뗀 것처럼 처리
    private void SimulateButtonUp(Button button)
    {
        ExecuteEvents.Execute(
            button.gameObject,
            new PointerEventData(EventSystem.current),
            ExecuteEvents.pointerUpHandler);

        ExecuteEvents.Execute(
            button.gameObject,
            new PointerEventData(EventSystem.current),
            ExecuteEvents.pointerClickHandler);
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
        // 일시정지 처리
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

    public void Display() => gameObject.SetActive(true);
    public void Hide()    => gameObject.SetActive(false);
}
using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;
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

    public async Task Initialize(StageModel model)
    {
        addressableManager = SystemsManager.Get<IAddressableManager>();
        cameraSystem = SystemsManager.Get<ICameraSystem>();

        Vector3 startPos = model.StartPosition.ToVector3();
        Quaternion startRot = Quaternion.Euler(model.StartRotation.ToVector3());

        GameObject stage = await addressableManager.InstantiateAsync(model.PrefabPath);
        GameObject car = await addressableManager.InstantiateAsync("Car", startPos, startRot);
        GameObject miniMap = await addressableManager.InstantiateAsync("MiniMap");
        cameraSystem.ChaseTarget(car.transform, 5,1,20,20);
        //cameraSystem.SetZoomSmooth(5,10);
        miniMap.GetComponent<MinimapSystem>().SetTarget(car.transform);
    }

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
}

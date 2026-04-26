using System;
using System.Threading.Tasks;
using JsonModel;
using UnityEngine;

public class IngameView : MonoBehaviour, IIngameView
{
    private IAddressableManager addressableManager;

    public async Task Initialize(StageModel model)
    {
        addressableManager = SystemsManager.Get<IAddressableManager>();

        Vector3 startPos = model.StartPosition.ToVector3();
        Quaternion startRot = Quaternion.Euler(model.StartRotation.ToVector3());

        GameObject stage = await addressableManager.InstantiateAsync(model.PrefabPath);
        GameObject car = await addressableManager.InstantiateAsync("Car", startPos, startRot);
        GameObject miniMap = await addressableManager.InstantiateAsync("MiniMap");

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

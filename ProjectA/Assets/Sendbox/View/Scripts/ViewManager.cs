using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class ViewManager
{
    private static bool isInitialized;
    private static Canvas rootCanvas;
    private static readonly Dictionary<Type, MonoBehaviour> instances = new();

    public static async Task Initialize()
    {
        if (isInitialized) return;

        CreateRootCanvas();
        CreateViews();
        isInitialized = true;
    }

    private static void CreateRootCanvas()
    {
        var canvasObject = new GameObject("ViewManager_Canvas");
        GameObject.DontDestroyOnLoad(canvasObject);

        rootCanvas = canvasObject.AddComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        rootCanvas.sortingOrder = 0;

        var scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2560, 1440);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    private static void CreateViews()
    {
        Register<ILobbyView, LobbyView>();
        Register<ILoadingView, LoadingView>();
        Register<ISystemPopupView, SystemPopupView>();
        Register<IInventoryView, InventoryView>();
        Register<IShopView, ShopView>();
        Register<ISkillView, SkillView>();
        Register<ICharactersView, CharactersView>();
        Register<IOptionView, OptionView>();
        Register<IStageSelectionView, StageSelectionView>();
        Register<IIngameView, IngameView>();
        Register<IPauseView, PauseView>();
    }

    private static void Register<TInterface, TConcrete>()
        where TInterface : IView
        where TConcrete : MonoBehaviour, TInterface
    {
        var interfaceType = typeof(TInterface);
        var concreteName = typeof(TConcrete).Name;

        if (instances.ContainsKey(interfaceType))
        {
            Debug.LogWarning($"[ViewManager] {interfaceType.Name}은 이미 등록되어 있습니다.");
            return;
        }

        var prefab = Resources.Load<TConcrete>(concreteName);
        if (prefab == null)
        {
            Debug.LogError($"[ViewManager] Resources/{concreteName} 프리팹을 찾을 수 없습니다.");
            return;
        }

        var instance = GameObject.Instantiate(prefab, rootCanvas.transform);

        // RectTransform을 캔버스에 꽉 차게 설정
        var rect = instance.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        instance.Hide();
        instances[interfaceType] = instance;
    }

    public static TInterface Get<TInterface>() where TInterface : class, IView
    {
        var interfaceType = typeof(TInterface);

        if (instances.TryGetValue(interfaceType, out var instance))
            return instance as TInterface;

        Debug.LogError($"[ViewManager] {interfaceType.Name}이 등록되지 않았습니다.");
        return null;
    }

    public static bool IsInitialized() => isInitialized;

    public static void Dispose()
    {
        foreach (var instance in instances.Values)
        {
            if (instance != null)
                GameObject.Destroy(instance.gameObject);
        }

        instances.Clear();

        if (rootCanvas != null)
            GameObject.Destroy(rootCanvas.gameObject);

        rootCanvas = null;
        isInitialized = false;
    }
}
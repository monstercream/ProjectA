using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class ViewManager
{
    private static bool isInitialized;

    private static readonly Dictionary<Type, MonoBehaviour> instances = new();

    public static async Task Initialize()
    {
        if (isInitialized) return;

        CreateViews();
        isInitialized = true;
    }

    private static void CreateViews()
    {
        Register<ILobbyView, LobbyView>();
        Register<ILoadingView, LoadingView>();
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

        var instance = GameObject.Instantiate(prefab);
        GameObject.DontDestroyOnLoad(instance);
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
        isInitialized = false;
    }
}
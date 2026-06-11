using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressableManager : IAddressableManager
{
    [ShowInInspector] private Dictionary<string, AsyncOperationHandle> _handles = new();

    private Dictionary<GameObject, AsyncOperationHandle<GameObject>> _instanceHandles = new();

    // ─── 단일 에셋 로드 ───────────────────────────────────────
    public async Task<T> LoadAssetAsync<T>(string key) where T : class
    {
        if (_handles.TryGetValue(key, out var cached))
        {
            if (cached.Result is T result)
            {
                Debug.Log($"[AddressableManager] Cache hit: '{key}'");
                return result;
            }
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _handles[key] = handle;
            Debug.Log($"[AddressableManager] Loaded: '{key}'");
            return handle.Result;
        }

        Debug.LogError($"[AddressableManager] Load failed: '{key}'. Error: {handle.OperationException?.Message}");
        Addressables.Release(handle);
        return null;
    }

    // ─── 라벨로 여러 에셋 로드 ───────────────────────────────
    public async Task<IList<T>> LoadAssetsAsync<T>(string label) where T : class
    {
        if (_handles.TryGetValue(label, out var cached))
        {
            if (cached.Result is IList<T> results)
            {
                Debug.Log($"[AddressableManager] Cache hit (label): '{label}'");
                return results;
            }
        }

        var handle = Addressables.LoadAssetsAsync<T>(
            label,
            obj => Debug.Log($"[AddressableManager] Asset loaded: {obj}")
        );

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _handles[label] = handle;
            Debug.Log($"[AddressableManager] Label '{label}' — {handle.Result.Count} assets loaded");
            return handle.Result;
        }

        Debug.LogError(
            $"[AddressableManager] Label load failed: '{label}'. Error: {handle.OperationException?.Message}");
        Addressables.Release(handle);
        return null;
    }

    // ─── 씬 로드 ─────────────────────────────────────────────
    public async Task LoadSceneAsync(string key, bool additive = false)
    {
        var mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        var handle = Addressables.LoadSceneAsync(key, mode);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _handles[key] = handle;
            Debug.Log($"[AddressableManager] Scene loaded: '{key}'");
        }
        else
        {
            Debug.LogError(
                $"[AddressableManager] Scene load failed: '{key}'. Error: {handle.OperationException?.Message}");
        }
    }

    public async Task<GameObject> InstantiateAsync(string key, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(key, parent);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _instanceHandles[handle.Result] = handle; // GameObject 자체를 키로
            Debug.Log($"[AddressableManager] Instantiated: '{key}'");
            return handle.Result;
        }

        Debug.LogError(
            $"[AddressableManager] Instantiate failed: '{key}'. Error: {handle.OperationException?.Message}");
        return null;
    }

    public async Task<GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation,
        Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(key, position, rotation, parent);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _instanceHandles[handle.Result] = handle; // GameObject 자체를 키로
            Debug.Log($"[AddressableManager] Instantiated: '{key}' at {position}");
            return handle.Result;
        }

        Debug.LogError($"[AddressableManager] Instantiate failed: '{key}'");
        return null;
    }

    public Task ReleaseInstanceAsync(GameObject instance)
    {
        if (instance == null) return Task.CompletedTask;

        if (!_instanceHandles.TryGetValue(instance, out var handle))
        {
            Debug.LogWarning($"[AddressableManager] ReleaseInstance failed — not found: {instance.name}");
            Object.Destroy(instance);
            return Task.CompletedTask;
        }

        var savedHandle = handle;
        _instanceHandles.Remove(instance);
        Object.Destroy(instance);

        if (savedHandle.IsValid())
            Addressables.ReleaseInstance(savedHandle);

        Debug.Log($"[AddressableManager] Released instance: {instance.name}");
        return Task.CompletedTask;
    }

    public void ReleaseAll()
    {
        foreach (var (key, handle) in _handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
            Debug.Log($"[AddressableManager] Released: '{key}'");
        }

        _handles.Clear();

        foreach (var (instance, handle) in _instanceHandles)
        {
            if (handle.IsValid())
                Addressables.ReleaseInstance(handle);
            Debug.Log($"[AddressableManager] Released instance: {instance?.name}");
        }

        _instanceHandles.Clear();

        Debug.Log("[AddressableManager] All assets released");
    }

    // ─── ISystem Dispose ─────────────────────────────────────
    public void Dispose()
    {
        ReleaseAll();
    }
}
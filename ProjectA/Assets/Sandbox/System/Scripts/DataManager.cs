using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataManager : IDataManager
{
    [ShowInInspector] private Dictionary<string, string> jsonDatas = new();
    public void SetData(string title, string jsonData) => jsonDatas[title] = jsonData;
    private readonly Dictionary<string, object> cache = new();

    public Dictionary<string, T> GetDataDictionary<T>(string title)
    {
        var key = $"{title}:{typeof(T).Name}";
        if (cache.TryGetValue(key, out var cached))
            return cached as Dictionary<string, T>;

        if (!jsonDatas.TryGetValue(title, out var json))
        {
            Debug.LogError($"[DataManager] Key not found: '{title}'");
            return null;
        }

        var dict = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
        cache[key] = dict;
        return dict;
    }

    public T GetData<T>(string title, string id)
    {
        var dict = GetDataDictionary<T>(title);
        if (dict == null || !dict.TryGetValue(id, out var item))
        {
            Debug.LogError($"[DataManager] {title}[{id}] not found");
            return default;
        }

        return item;
    }

    public void Dispose()
    {
    }
}

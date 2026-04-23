using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataManager : IDataManager
{
    [ShowInInspector] private Dictionary<string, string> jsonDatas = new();
    public void SetData(string title, string jsonData) => jsonDatas[title] = jsonData;
    public Dictionary<string, T> GetDataDictionary<T>(string title)
    {
        if (!jsonDatas.ContainsKey(title))
        {
            Debug.LogError($"[DataManager] Key not found: '{title}'. Available keys: {string.Join(", ", jsonDatas.Keys)}");
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonDatas[title]);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] Failed to deserialize '{title}' as Dictionary<string, {typeof(T).Name}>. Error: {e.Message}");
            return null;
        }
    }

    public T GetData<T>(string title, string id)
    {
        return GetDataDictionary<T>(title)[id];
    }

    public void Dispose()
    {
    }
}

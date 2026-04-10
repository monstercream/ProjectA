using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class DataManager : IDataManager
{
    [ShowInInspector] private Dictionary<string, string> jsonDatas = new();
    public void SetData(string title, string jsonData) => jsonDatas[title] = jsonData;

    public Dictionary<string, T> GetDataDictionary<T>(string title)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonDatas[title]);
    }

    public T GetData<T>(string title, string id)
    {
        return GetDataDictionary<T>(title)[id];
    }

    public void Dispose()
    {
    }
}

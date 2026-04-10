using System.Collections.Generic;

public interface IDataManager : ISystem
{
    public void SetData(string title, string jsonData);
    public T GetData<T>(string title, string id);
    public Dictionary<string, T> GetDataDictionary<T>(string title);
}
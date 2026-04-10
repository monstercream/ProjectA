public interface ILoadingSystem : ISystem
{
    public void ExecuteCloudScript(string functionName, object functionParameter);
}
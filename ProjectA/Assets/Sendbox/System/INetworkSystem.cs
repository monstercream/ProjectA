using System.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;

public interface INetworkSystem : ISystem
{
    public Task<LoginResult> Login(string titleID, string deviceId);
    public Task<ExecuteFunctionResult> ExecuteScript(string functionName, object functionParameter = null);
    public Task<GetTitleDataResult> TitleData(string[] keys = null);
    public Task<GetUserDataResult> UserData(string[] keys = null);
    public Task<GetUserInventoryResult> Inventory();
}
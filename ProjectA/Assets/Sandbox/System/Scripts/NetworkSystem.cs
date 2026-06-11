using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayFab.CloudScriptModels;
using UnityEngine;

public class NetworkSystem : INetworkSystem
{
    private const int MaxRetryCount = 3;
    private const float RetryDelaySeconds = 2f;

    private Dictionary<string, string> titleData;
    private Dictionary<string, UserDataRecord> userData;
    private List<ItemInstance> inventory;
    private Dictionary<string, int> virtualCurrency;

    // apiCall이 Task<T>를 직접 반환하도록 변경 — TCS를 외부로 노출할 필요 없음
    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> apiCall)
    {
        int retryCount = 0;

        while (true)
        {
            try
            {
                return await apiCall();
            }
            catch (Exception e)
            {
                retryCount++;
                Debug.LogWarning($"API 호출 실패 (시도 {retryCount}/{MaxRetryCount}): {e.Message}");

                if (retryCount >= MaxRetryCount)
                {
                    Debug.LogError($"최대 재시도 횟수 초과: {e.Message}");
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds * retryCount));
            }
        }
    }

    // PlayFab 콜백 → Task<T> 변환을 각 메서드 안에서 직접 처리
    // TCS 생성과 반환이 같은 메서드 안에 있어서 흐름이 명확함
    public Task<LoginResult> Login(string titleID, string deviceId)
    {
        return ExecuteWithRetry<LoginResult>(() =>
        {
            var tcs = new TaskCompletionSource<LoginResult>();

            PlayFabClientAPI.LoginWithIOSDeviceID(
                new LoginWithIOSDeviceIDRequest
                {
                    TitleId = titleID,
                    DeviceId = deviceId,
                    CreateAccount = false,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true,
                        GetUserVirtualCurrency = true,
                        GetUserAccountInfo = true
                    }
                },
                res =>
                {
                    Debug.Log($"<size=15><color=#ff0000ff> Login Success </color></size><b>{res.PlayFabId}</b>");
                    tcs.SetResult(res);
                },
                error =>
                {
                    Debug.LogError($"Login Failed: {error.Error}");
                    Debug.LogError($"Error Details: {error.ErrorDetails}");
                    Debug.LogError($"Error Message: {error.ErrorMessage}");
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                }
            );

            return tcs.Task;
        });
    }

    public Task<ExecuteFunctionResult> ExecuteScript(string functionName, object functionParameter = null)
    {
        return ExecuteWithRetry<ExecuteFunctionResult>(() =>
        {
            var tcs = new TaskCompletionSource<ExecuteFunctionResult>();

            PlayFabCloudScriptAPI.ExecuteFunction(
                new ExecuteFunctionRequest
                {
                    FunctionName = functionName,
                    FunctionParameter = functionParameter,
                    GeneratePlayStreamEvent = true
                },
                result =>
                {
                    Debug.LogWarning(result.FunctionName);
                    tcs.SetResult(result);
                },
                error =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                }
            );

            return tcs.Task;
        });
    }

    public Task<GetTitleDataResult> TitleData(string[] keys = null)
    {
        return ExecuteWithRetry<GetTitleDataResult>(() =>
        {
            var tcs = new TaskCompletionSource<GetTitleDataResult>();

            PlayFabClientAPI.GetTitleData(
                new GetTitleDataRequest { Keys = keys?.ToList() },
                res =>
                {
                    titleData = res.Data;
                    tcs.SetResult(res);
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return tcs.Task;
        });
    }

    public Task<GetUserDataResult> UserData(string[] keys = null)
    {
        return ExecuteWithRetry<GetUserDataResult>(() =>
        {
            var tcs = new TaskCompletionSource<GetUserDataResult>();

            PlayFabClientAPI.GetUserData(
                new GetUserDataRequest { Keys = keys?.ToList() },
                res =>
                {
                    userData = res.Data;
                    tcs.SetResult(res);
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return tcs.Task;
        });
    }

    public Task<GetUserInventoryResult> Inventory()
    {
        return ExecuteWithRetry<GetUserInventoryResult>(() =>
        {
            var tcs = new TaskCompletionSource<GetUserInventoryResult>();

            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                result =>
                {
                    inventory = result.Inventory;
                    virtualCurrency = result.VirtualCurrency;
                    tcs.SetResult(result);
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return tcs.Task;
        });
    }

    public void Dispose() { }
}
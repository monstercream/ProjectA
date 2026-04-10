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
    private async Task<T> ExecuteWithRetry<T>(Func<TaskCompletionSource<T>, Task> apiCall)
    {
        int retryCount = 0;

        while (true)
        {
            var tcs = new TaskCompletionSource<T>();

            await apiCall(tcs);

            try
            {
                return await tcs.Task;
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

                await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds * retryCount)); // 점진적 딜레이
            }
        }
    }

    public async Task<LoginResult> Login(string titleID, string deviceId)
    {
        return await ExecuteWithRetry<LoginResult>(tcs =>
        {
            GetPlayerCombinedInfoRequestParams requestParams = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
                GetUserVirtualCurrency = true,
                GetUserAccountInfo = true
            };

            LoginWithIOSDeviceIDRequest request = new LoginWithIOSDeviceIDRequest
            {
                TitleId = titleID,
                DeviceId = deviceId,
                CreateAccount = false,
                InfoRequestParameters = requestParams
            };

            PlayFabClientAPI.LoginWithIOSDeviceID(
                request,
                (res) =>
                {
                    Debug.Log($"<size=15><color=#ff0000ff> Login Success </color></size><b>{res.PlayFabId}</b>");
                    tcs.SetResult(res);
                },
                (error) =>
                {
                    Debug.LogError($"Login Failed: {error.Error}");
                    Debug.LogError($"Error Details: {error.ErrorDetails}");
                    Debug.LogError($"Error Message: {error.ErrorMessage}");
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                }
            );

            return Task.CompletedTask;
        });
    }

    public async Task<ExecuteFunctionResult> ExecuteScript(string functionName, object functionParameter = null)
    {
        return await ExecuteWithRetry<ExecuteFunctionResult>(tcs =>
        {
            var request = new ExecuteFunctionRequest
            {
                FunctionName = functionName,
                FunctionParameter = functionParameter,
                GeneratePlayStreamEvent = true
            };

            PlayFabCloudScriptAPI.ExecuteFunction(
                request,
                (result) =>
                {
                    Debug.LogWarning(result.FunctionName);
                    tcs.SetResult(result);
                },
                (error) =>
                {
                    Debug.LogError(error.GenerateErrorReport());
                    tcs.SetException(new Exception(error.GenerateErrorReport()));
                }
            );

            return Task.CompletedTask;
        });
    }

    public async Task<GetTitleDataResult> TitleData(string[] keys = null)
    {
        return await ExecuteWithRetry<GetTitleDataResult>(tcs =>
        {
            var data = new GetTitleDataRequest()
            {
                Keys = keys == null ? null : keys.ToList()
            };

            PlayFabClientAPI.GetTitleData(
                data,
                res =>
                {
                    tcs.SetResult(res);
                    titleData = res.Data;
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return Task.CompletedTask;
        });
    }

    public async Task<GetUserDataResult> UserData(string[] keys = null)
    {
        return await ExecuteWithRetry<GetUserDataResult>(tcs =>
        {
            var data = new GetUserDataRequest()
            {
                Keys = keys == null ? null : keys.ToList()
            };

            PlayFabClientAPI.GetUserData(
                data,
                res =>
                {
                    tcs.SetResult(res);
                    userData = res.Data;
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return Task.CompletedTask;
        });
    }

    public async Task<GetUserInventoryResult> Inventory()
    {
        return await ExecuteWithRetry<GetUserInventoryResult>(tcs =>
        {
            var request = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(
                request,
                result =>
                {
                    tcs.SetResult(result);
                    inventory = result.Inventory;
                    virtualCurrency = result.VirtualCurrency;
                },
                error => tcs.SetException(new Exception(error.GenerateErrorReport()))
            );

            return Task.CompletedTask;
        });
    }

    public void Dispose()
    {
    }
}
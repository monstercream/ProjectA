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
	private Dictionary<string, string> titleData;
	private Dictionary<string, UserDataRecord> userData;
	private List<ItemInstance> inventory;
	private Dictionary<string, int> virtualCurrency;

	public async Task<LoginResult> Login(string titleID, string deviceId)
	{
		var tcs = new TaskCompletionSource<LoginResult>();

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
				tcs.SetException(new System.Exception(error.GenerateErrorReport()));
			}
		);

		return await tcs.Task;
	}

	public async Task<ExecuteFunctionResult> ExecuteScript(string functionName, object functionParameter = null)
	{
		var tcs = new TaskCompletionSource<ExecuteFunctionResult>();

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

		return await tcs.Task;
	}

	public async Task<GetTitleDataResult> TitleData(string[] keys = null)
	{
		var tcs = new TaskCompletionSource<GetTitleDataResult>();

		var data = new GetTitleDataRequest()
		{
			Keys = keys == null ? null : keys.ToList()
		};

		PlayFabClientAPI.GetTitleData(data, res =>
			{
				tcs.SetResult(res);
				titleData = res.Data;
			},
			(error) => { tcs.SetException(new Exception("An error occurred" + error)); });
		return null;
	}

	public async Task<GetUserDataResult> UserData(string[] keys = null)
	{
		var tcs = new TaskCompletionSource<GetUserDataResult>();

		var data = new GetUserDataRequest()
		{
			Keys = keys == null ? null : keys.ToList()
		};

		PlayFabClientAPI.GetUserData(data, res =>
			{
				tcs.SetResult(res);
				userData = res.Data;
			},
			(error) => { tcs.SetException(new Exception("An error occurred" + error)); });

		return null;
	}

	public async Task<GetUserInventoryResult> Inventory()
	{
		var tcs = new TaskCompletionSource<GetUserInventoryResult>();
		GetUserInventoryRequest request = new GetUserInventoryRequest();
		// PlayFabClientAPI.GetUserInventory(request, result =>
		// {
		// 	tcs.SetResult(result);
		// 	inventory = result.Inventory;
		// 	virtualCurrency = result.VirtualCurrency;
		// });

		return null;
	}

	public void Dispose()
	{
	}
}
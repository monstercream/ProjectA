using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
	public class SystemsManager : IDisposable
	{
		private static bool isInitialized;

		private static readonly Dictionary<Type, object> systems = new Dictionary<Type, object>();

		public static async Task Initialize()
		{
			if (!isInitialized)
			{
				CreateSystems();
				//await LoadSystemsAsync();
				isInitialized = true;
			}
		}

		private static void CreateSystems()
		{
			RegisterMainSystem<INetworkSystem>(new NetworkSystem());
			RegisterMainSystem<ILoadingSystem>(new LoadingSystem());
			RegisterMainSystem<IDataManager>(new DataManager());
		}

		private static void RegisterMainSystem<T>(ISystem system) where T : ISystem
		{
			systems[typeof(T)] = system;
		}


		public static T Get<T>() where T : class, ISystem
		{
			return (T) systems[typeof(T)] as T;
		}

		public static bool IsInitialized() => isInitialized;

		public void Dispose()
		{
			Get<IDataManager>().Dispose();
			Get<INetworkSystem>().Dispose();

			systems.Clear();
		}
	}
}


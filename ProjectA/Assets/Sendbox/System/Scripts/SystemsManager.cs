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
				isInitialized = true;
			}
		}

		private static void CreateSystems()
		{
			Register<INetworkSystem>(new NetworkSystem());
			Register<IDataManager>(new DataManager());
			Register<IAddressableManager>(new AddressableManager());
			Register<ICameraSystem>(new CameraSystem());
		}

		private static void Register<T>(ISystem system) where T : ISystem
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
			Get<IAddressableManager>().Dispose();

			systems.Clear();
		}
	}
}


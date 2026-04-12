using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
	public class ViewManager : IDisposable
	{
		private static bool isInitialized;

		private static readonly Dictionary<Type, object> views = new Dictionary<Type, object>();
		

		public static async Task Initialize()
		{
			if (!isInitialized)
			{
				CreateViews();
				isInitialized = true;
			}
		}

		private static void CreateViews()
		{
			RegisterMainSystem<ILobbyView>(new NewLobbyView());
		}

		private static void RegisterMainSystem<T>(IView view) where T : IView
		{
			views[typeof(T)] = view;
		}


		public static T Get<T>() where T : class, IView
		{
			return (T) views[typeof(T)] as T;
		}

		public static bool IsInitialized() => isInitialized;

		public void Dispose()
		{
			Get<ILobbyView>().Dispose();

			views.Clear();
		}
	}
}


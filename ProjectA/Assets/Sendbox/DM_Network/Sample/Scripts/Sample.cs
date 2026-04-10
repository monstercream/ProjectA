using UnityEngine;

namespace DM_Network
{
    public class Sample : MonoBehaviour
    {
        private DM_Network network;

        private async void Awake()
        {
            network = new DM_Network();
            if (!await network.LoginAsync("7789B", "42779113-0012-58F2-939B-0870AFAE582E"))
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.RequestAsync("ServerTest"))
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.TitleDataRequestAsync())
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.UserDataRequestAsync())
            {
                Debug.LogWarning($"Failed");
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DM_Network
{
    public interface INetwork
    {
        public Task<bool> LoginAsync(string titleID, string loginID);
        public Task<bool> RequestAsync(string functionName, Dictionary<string, string> functionParameter);
        public Task<bool> UserDataRequestAsync(string[] keys);
        public Task<bool> TitleDataRequestAsync(string[] keys);
    }

    public class UserData
    {
        public static string DisplayName { get; set; }
        public static string PlayerID { get; set; }
        public static string PlayerIconPath { get; set; }
        public static bool NewCreated { get; set; }
    }
}
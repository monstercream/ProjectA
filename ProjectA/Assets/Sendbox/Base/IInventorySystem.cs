using System.Threading.Tasks;
using PlayFab.ClientModels;

public interface IInventorySystem : ISystem
{
    public Task PurchaseItem(string itemID, int amount = 1);
    public Task RemoveItem(string itemID, int amount = 1);
    public Task GrantItem(string itemID, int amount = 1);
}


using System.Threading.Tasks;
using PlayFab.ClientModels;

public class InventorySystem : IInventorySystem
{
    private NetworkSystem networkSystem;

    public InventorySystem(NetworkSystem networkSystem)
    {
        this.networkSystem = networkSystem;
        networkSystem.Inventory();
    }

    public Task PurchaseItem(string itemID, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public Task RemoveItem(string itemID, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public Task GrantItem(string itemID, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
}

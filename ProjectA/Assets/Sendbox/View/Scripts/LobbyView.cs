using System.Threading.Tasks;
using UnityEngine;

public class LobbyView : MonoBehaviour, ILobbyView
{
    private ISystemPopupView systemPopupView;

    public void Display() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public async Task  Start()
    {
        systemPopupView = ViewManager.Get<ISystemPopupView>();
        systemPopupView.Initialize("Notice", "Test");
        systemPopupView.Display();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

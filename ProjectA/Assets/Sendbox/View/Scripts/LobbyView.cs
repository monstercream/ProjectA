using UnityEngine;

public class LobbyView : MonoBehaviour, ILobbyView
{
    public void Display() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void Dispose()
    {
        Destroy(gameObject);
    }
}

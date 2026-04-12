using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IInventoryView
{
    [SerializeField] private Button closeButton;
    public void Dispose()
    {
    }

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public abstract class BaseView : MonoBehaviour, IView
{
    public bool IsVisible => gameObject.activeSelf;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
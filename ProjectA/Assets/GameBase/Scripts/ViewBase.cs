using UnityEngine;

public abstract class BaseView<T> : MonoBehaviour where T : BaseView<T>
{
    private static T view;

    protected abstract string ViewName { get; }

    public static T View
    {
        get
        {
            if (view == null)
            {
                T prefab = Resources.Load<T>(GetViewName());
                if (prefab == null)
                {
                    Debug.LogError($"Resources에서 {GetViewName()}을 찾을 수 없습니다.");
                    return null;
                }

                view = GameObject.Instantiate(prefab);
                Object.DontDestroyOnLoad(view);
            }

            return view;
        }
    }

    private static string GetViewName()
    {
        return typeof(T).Name;
    }

    public virtual void Show()
    {
        View?.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        View?.gameObject.SetActive(false);
    }

    public virtual void Dispose()
    {
        if (view != null)
        {
            Object.Destroy(view.gameObject);
            view = null;
        }
    }
}
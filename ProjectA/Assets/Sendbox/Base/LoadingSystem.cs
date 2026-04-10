using UnityEngine;

public class LoadingSystem : ILoadingSystem
{
    const string VIEW_NAME = "LoadingView";

    private LoadingView view;

    private LoadingView View
    {
        get
        {
            if (view == null)
            {
                LoadingView prefab = Resources.Load<LoadingView>(VIEW_NAME);
                if (prefab == null)
                {
                    Debug.LogError($"Resources에서 {VIEW_NAME}을 찾을 수 없습니다.");
                    return null;
                }

                LoadingView go = GameObject.Instantiate(prefab);
                Object.DontDestroyOnLoad(go);
                view = go.GetComponent<LoadingView>();
            }

            return view;
        }
    }

    public void SetValue(float value)
    {
        View?.SetLoading(value);
    }

    public void Show()
    {
        View?.gameObject.SetActive(true);
    }

    public void Hide()
    {
        View?.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        if (view != null)
        {
            Object.Destroy(view.gameObject);
            view = null;
        }
    }
}
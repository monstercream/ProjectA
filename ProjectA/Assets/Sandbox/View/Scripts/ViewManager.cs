using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    private readonly Dictionary<Type, BaseView> views = new();

    public static ViewManager GetOrCreate()
    {
        if (Instance != null) return Instance;

        var existing = FindObjectOfType<ViewManager>(true);  // 비활성 오브젝트도 검색
        if (existing != null)
        {
            existing.gameObject.SetActive(true);  // 비활성이었다면 활성화 → Awake 실행됨
            return existing;
        }

        var go = new GameObject("ViewManager");
        DontDestroyOnLoad(go);
        return go.AddComponent<ViewManager>();  // AddComponent 시점에 Awake 즉시 실행
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RegisterAllViewsInScene();
    }

    // SetParent 제거 — UI(RectTransform)를 Canvas 밖으로 옮기면
    // 화면에서 사라지고 레이아웃이 깨지므로 딕셔너리 등록만 한다
    private void RegisterAllViewsInScene()
    {
        var allViews = FindObjectsOfType<BaseView>(true);
        Debug.Log($"[ViewManager] 씬에서 발견한 View 수: {allViews.Length}");

        foreach (var view in allViews)
        {
            views[view.GetType()] = view;
            Debug.Log($"[ViewManager] 등록: {view.GetType().Name}");
        }
    }

    // 동적 생성 View 수동 등록용
    public void Register(BaseView view)
    {
        views[view.GetType()] = view;
    }

    public T Show<T>() where T : BaseView
    {
        if (views.TryGetValue(typeof(T), out var view))
        {
            view.Show();
            return view as T;
        }

        Debug.LogError($"[ViewManager] View not found: {typeof(T).Name}");
        return null;
    }

    public void Hide<T>() where T : BaseView
    {
        if (views.TryGetValue(typeof(T), out var view))
            view.Hide();
    }

    public void HideAll()
    {
        foreach (var view in views.Values)
            view.Hide();
    }

    public T Get<T>() where T : BaseView
    {
        if (!views.TryGetValue(typeof(T), out var view))
            Debug.LogError($"[ViewManager] View not found: {typeof(T).Name}");

        return view as T;
    }
}
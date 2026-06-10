using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    private Dictionary<Type, BaseView> views = new();

    public static ViewManager GetOrCreate()
    {
        if (Instance != null) return Instance;

        // 씬에 이미 있으면 찾아서 반환
        var existing = FindObjectOfType<ViewManager>();
        if (existing != null) return existing;

        // 없으면 자동 생성
        var go = new GameObject("ViewManager");
        DontDestroyOnLoad(go);
        return go.AddComponent<ViewManager>();
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

    // 씬 전체에서 BaseView를 찾아 자식으로 편입 후 등록
    private void RegisterAllViewsInScene()
    {
        var allViews = FindObjectsOfType<BaseView>(true);

        foreach (var view in allViews)
        {
            // 이미 내 자식이면 스킵
            if (view.transform.parent == transform) continue;

            // 자식으로 편입
            view.transform.SetParent(transform, worldPositionStays: false);
        }

        // 자식 등록
        foreach (var view in GetComponentsInChildren<BaseView>(true))
        {
            views[view.GetType()] = view;
            Debug.Log($"[ViewManager] 등록: {view.GetType().Name}");
        }
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
        views.TryGetValue(typeof(T), out var view);
        return view as T;
    }
}
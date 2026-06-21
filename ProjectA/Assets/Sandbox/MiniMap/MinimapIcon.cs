using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private RectTransform iconRect;

    private Camera _minimapCamera;   // Camera를 직접 캐싱

    public Transform SetTarget { set => target = value; }

    public void Initialize(MinimapCamera cam)        // 주입 + 1회 캐싱
    {
        _minimapCamera = cam.GetComponent<Camera>();
    }

    private void LateUpdate()                         // Update() 통째로 제거
    {
        if (target == null || _minimapCamera == null) return;

        Vector3 v = _minimapCamera.WorldToViewportPoint(target.position);
        iconRect.anchoredPosition = new Vector2(
            (v.x - 0.5f) * minimapRect.sizeDelta.x,
            (v.y - 0.5f) * minimapRect.sizeDelta.y);
        iconRect.rotation = Quaternion.Euler(0f, 0f, -target.eulerAngles.y);
    }
}
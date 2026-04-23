using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private Transform target;          // 자동차
    [SerializeField] private RectTransform minimapRect; // 미니맵 UI 크기
    [SerializeField] private RectTransform iconRect;    // 아이콘 UI

    private MinimapCamera _minimapCam;

    public Transform SetTarget
    {
        set => target = value;
    }
    private void Update()
    {
        if (_minimapCam == null)
            _minimapCam = FindObjectOfType<MinimapCamera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 월드 좌표 → 미니맵 UI 좌표 변환
        Vector3 viewportPos = _minimapCam
            .GetComponent<Camera>()
            .WorldToViewportPoint(target.position);

        iconRect.anchoredPosition = new Vector2(
            (viewportPos.x - 0.5f) * minimapRect.sizeDelta.x,
            (viewportPos.y - 0.5f) * minimapRect.sizeDelta.y
        );

        // 자동차 방향 아이콘 회전
        iconRect.rotation = Quaternion.Euler(
            0f, 0f, -target.eulerAngles.y
        );
    }
}
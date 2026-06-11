using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // 자동차
    [SerializeField] private float height = 50f; // 카메라 높이
    [SerializeField] private float size = 30f; // 보이는 범위

    private Camera camera;

    public Transform SetTarget
    {
        set => target = value;
    }

    private void Awake()
    {
        camera = GetComponent<Camera>();
        camera.orthographicSize = size;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 자동차 위에서 따라다님
        transform.position = new Vector3(
            target.position.x,
            target.position.y + height,
            target.position.z
        );
    }
}
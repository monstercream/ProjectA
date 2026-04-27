// CameraDirector.cs
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    private Camera    cam;
    private Transform followTarget;
    private Vector3   followOffset = new Vector3(0f, 10f, -10f);

    // ── 줌 ───────────────────────────────────────────
    private float targetFov;
    private float currentFov;
    private float zoomSpeed;
    private bool  isZooming;

    // ── 흔들기 ───────────────────────────────────────
    private float shakeIntensity;
    private float shakeDuration;
    private float shakeTimer;

    // ── 체이스 ───────────────────────────────────────
    private bool    isChaseMode;
    private float   chaseDistance    = 10f;  // 타겟 뒤 거리
    private float   chaseHeight      =  5f;  // 타겟 위 높이
    private float   chaseRotSpeed    =  5f;  // 회전 따라가는 속도
    private float   chasePosSpeed    =  5f;  // 위치 따라가는 속도
    private Vector3 chaseVelocity;           // SmoothDamp용

    public bool  IsFollowing => followTarget != null;
    public float CurrentZoom => currentFov;

    private void Awake()
    {
        cam        = GetComponent<Camera>();
        currentFov = cam.fieldOfView;
        targetFov  = currentFov;
    }

    private void LateUpdate()
    {
        if (isChaseMode) HandleChase();
        else             HandleFollow();

        HandleZoom();
        HandleShake();
    }

    // ── 일반 팔로잉 ──────────────────────────────────
    private void HandleFollow()
    {
        if (followTarget == null) return;
        transform.position = followTarget.position + followOffset;
        transform.LookAt(followTarget);
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        isChaseMode  = false;
    }

    public void SetFollowOffset(Vector3 offset)
    {
        followOffset = offset;
    }

    // ── 체이스 카메라 ────────────────────────────────
    // 타겟의 forward 반대 방향(뒤쪽) + 높이 offset 위치를 목표로
    // SmoothDamp로 위치를 부드럽게 따라가고
    // 회전은 Slerp으로 타겟을 바라보도록 처리
    private void HandleChase()
    {
        if (followTarget == null) return;

        // 타겟 뒤쪽 + 높이 위치 계산
        Vector3 targetBack    = followTarget.position
                                - followTarget.forward * chaseDistance
                                + Vector3.up * chaseHeight;

        // 위치 — SmoothDamp로 부드럽게 추격
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetBack,
            ref chaseVelocity,
            1f / chasePosSpeed);

        // 회전 — 타겟을 바라보도록 Slerp
        Quaternion lookRot = Quaternion.LookRotation(
            followTarget.position - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            chaseRotSpeed * Time.deltaTime);
    }

    public void SetChaseMode(Transform target, float distance, float height, float posSpeed, float rotSpeed)
    {
        followTarget  = target;
        chaseDistance = distance;
        chaseHeight   = height;
        chasePosSpeed = posSpeed;
        chaseRotSpeed = rotSpeed;
        isChaseMode   = true;
        chaseVelocity = Vector3.zero;
    }

    public void StopChase()
    {
        isChaseMode = false;
    }

    // ── 줌 ───────────────────────────────────────────
    private void HandleZoom()
    {
        if (!isZooming) return;

        currentFov      = Mathf.MoveTowards(currentFov, targetFov, zoomSpeed * Time.deltaTime);
        cam.fieldOfView = currentFov;

        if (Mathf.Approximately(currentFov, targetFov))
            isZooming = false;
    }

    public void ZoomTo(float fov, float duration)
    {
        targetFov = fov;
        zoomSpeed = Mathf.Abs(targetFov - currentFov) / Mathf.Max(duration, 0.001f);
        isZooming = true;
    }

    public void ZoomImmediate(float fov)
    {
        targetFov       = fov;
        currentFov      = fov;
        cam.fieldOfView = fov;
        isZooming       = false;
    }

    // ── 흔들기 ───────────────────────────────────────
    private void HandleShake()
    {
        if (shakeTimer <= 0f) return;

        shakeTimer -= Time.deltaTime;

        float strength = shakeIntensity * (shakeTimer / shakeDuration);
        transform.position += Random.insideUnitSphere * strength;

        if (shakeTimer <= 0f)
            shakeTimer = 0f;
    }

    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration  = duration;
        shakeTimer     = duration;
    }
}
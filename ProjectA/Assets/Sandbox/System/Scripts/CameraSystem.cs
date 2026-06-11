// CameraSystem.cs
using UnityEngine;

public class CameraSystem : ICameraSystem
{
    private readonly float defaultFov = 40f;
    private readonly float minFov     = 15f;
    private readonly float maxFov     = 60f;

    private Camera         camera;
    private CameraDirector director;

    public bool  IsFollowing => director.IsFollowing;
    public float CurrentZoom => director.CurrentZoom;

    public CameraSystem()
    {
        BuildCamera();
    }

    private void BuildCamera()
    {
        var go = new GameObject("GameCamera");
        Object.DontDestroyOnLoad(go);

        camera               = go.AddComponent<Camera>();
        camera.fieldOfView   = defaultFov;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane  = 1000f;

        director = go.AddComponent<CameraDirector>();
    }

    public void FollowTarget(Transform target)
    {
        director.SetFollowTarget(target);
    }

    public void StopFollow()
    {
        director.SetFollowTarget(null);
    }

    // 체이스 모드 — 타겟 뒤에서 추격
    public void ChaseTarget(Transform target, float distance = 10f, float height = 5f,
                            float posSpeed = 5f, float rotSpeed = 5f)
    {
        director.SetChaseMode(target, distance, height, posSpeed, rotSpeed);
    }

    public void StopChase()
    {
        director.StopChase();
    }

    public void ZoomIn(float amount)
    {
        float target = Mathf.Clamp(director.CurrentZoom - amount, minFov, maxFov);
        director.ZoomTo(target, 0.3f);
    }

    public void ZoomOut(float amount)
    {
        float target = Mathf.Clamp(director.CurrentZoom + amount, minFov, maxFov);
        director.ZoomTo(target, 0.3f);
    }

    public void SetZoom(float fov)
    {
        director.ZoomImmediate(Mathf.Clamp(fov, minFov, maxFov));
    }

    public void SetZoomSmooth(float fov, float duration)
    {
        director.ZoomTo(Mathf.Clamp(fov, minFov, maxFov), duration);
    }

    public void SetOffset(Vector3 offset)
    {
        director.SetFollowOffset(offset);
    }

    public void Shake(float intensity, float duration)
    {
        director.Shake(intensity, duration);
    }

    public void Dispose()
    {
        if (camera != null) Object.Destroy(camera.gameObject);
    }
}
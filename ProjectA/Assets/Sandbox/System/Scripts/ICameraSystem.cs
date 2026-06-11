// ICameraSystem.cs
using UnityEngine;

public interface ICameraSystem : ISystem
{
    void FollowTarget(Transform target);
    void StopFollow();
    void ChaseTarget(Transform target, float distance = 10f, float height = 5f,
        float posSpeed = 5f, float rotSpeed = 5f);
    void StopChase();
    void ZoomIn(float amount);
    void ZoomOut(float amount);
    void SetZoom(float fov);
    void SetZoomSmooth(float fov, float duration);
    void SetOffset(Vector3 offset);
    void Shake(float intensity, float duration);
    bool IsFollowing { get; }
    float CurrentZoom { get; }
    void Dispose();
}
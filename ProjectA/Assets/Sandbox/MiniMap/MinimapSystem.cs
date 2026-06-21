using UnityEngine;

public class MinimapSystem : MonoBehaviour
{
    [SerializeField] private MinimapCamera _minimapCamera;
    [SerializeField] private MinimapIcon _minimapIcon;

    public void SetTarget(Transform target)
    {
        _minimapCamera.SetTarget = target;
        _minimapIcon.SetTarget = target;
        _minimapIcon.Initialize(_minimapCamera);   // 카메라 주입
    }
}

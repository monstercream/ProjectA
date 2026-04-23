using UnityEngine;

public class MinimapSystem : MonoBehaviour
{
    [SerializeField] private MinimapCamera _minimapCamera;
    [SerializeField] private MinimapIcon _minimapIcon;

    public void SetTarget(Transform target)
    {
        _minimapCamera.SetTarget = target;
        _minimapIcon.SetTarget = target;
    }
}

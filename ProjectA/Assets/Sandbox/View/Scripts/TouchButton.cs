using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 버튼을 "누르고 있는 동안" 입력으로 사용하기 위한 컴포넌트.
/// 좌/우 조작 버튼처럼 hold 입력이 필요한 곳에 부착.
/// </summary>
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>현재 눌려 있는지 여부.</summary>
    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData) => IsPressed = true;
    public void OnPointerUp(PointerEventData eventData)   => IsPressed = false;

    // 비활성화/파괴 시 "눌린 채" 남지 않도록 안전장치
    private void OnDisable() => IsPressed = false;
}
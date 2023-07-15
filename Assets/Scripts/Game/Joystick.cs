using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform joystickBackground;
    private RectTransform joystickHandle;

    private Vector2 inputDirection = Vector2.zero;
    private bool isJoystickActive = false;

    public float handleLimit = 1f;

    private void Start()
    {
        joystickBackground = GetComponent<RectTransform>();
        joystickHandle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(null, joystickBackground.position);
        Vector2 radius = joystickBackground.sizeDelta / 2f;
        inputDirection = (eventData.position - position) / (radius * handleLimit);

        inputDirection = Vector2.ClampMagnitude(inputDirection, 1f);

        joystickHandle.anchoredPosition = inputDirection * radius * handleLimit;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isJoystickActive = true;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        isJoystickActive = false;
    }

    public float Horizontal => inputDirection.x;
    public float Vertical => inputDirection.y;

    public bool IsJoystickActive => isJoystickActive;
}

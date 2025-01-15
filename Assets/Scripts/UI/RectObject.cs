using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectObject : MonoBehaviour
{
    public RectTransform RectTransform => transform as RectTransform;
    public Vector2 AnchoredPosition
    {
        get => RectTransform.anchoredPosition;
        set => RectTransform.anchoredPosition = value;
    }
    public Vector2 LocalPositon
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
    public Rect Rect => RectTransform.rect;
    public float Width => RectTransform.sizeDelta.x;
    public float Height => RectTransform.sizeDelta.y;
    public float HalfWidth => Width / 2;
    public float HalfHeight => Height / 2;
    public float Top => transform.localPosition.y + HalfHeight;
    public float Bottom => transform.localPosition.y - HalfHeight;
    public float Left => transform.localPosition.x - HalfWidth;
    public float Right => transform.localPosition.x + HalfWidth;

    protected virtual void Awake() { }

    protected virtual void Start() { }

    public bool IsInsideRect(RectTransform rectTransform)
    {
        Transform parent = rectTransform.parent;
        rectTransform.SetParent(transform);

        Rect rect = rectTransform.rect;
        Vector2 localPosition = rectTransform.localPosition;
        
        float x = localPosition.x;
        float y = localPosition.y;

        float halfWidth = rect.width / 2;
        float halfHeight = rect.height / 2;

        float top = y + halfHeight;
        float bottom = y - halfHeight;
        float left = x - halfWidth;
        float right = x + halfWidth;

        bool isInside = left >= Rect.x && right <= Rect.xMax && bottom >= Rect.y && top <= Rect.yMax;

        rectTransform.SetParent(parent);

        return isInside;
    }

    public bool IsInsideRect(RectObject rect) => IsInsideRect(rect.RectTransform);

    public bool IsScreenPositionInsideRect(Vector2 screenPosition) => RectTransformUtility.RectangleContainsScreenPoint(RectTransform, screenPosition);
}

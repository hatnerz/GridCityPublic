using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverDelay = 0.3f;
    public float scaleFactor = 1.3f;
    public Vector2 moveOnHover = new Vector2(0, 90);
    public float scaleSpeed = 25f;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Coroutine hoverCoroutine;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        var newPosition = new Vector2(originalPosition.x + moveOnHover.x, originalPosition.y + moveOnHover.y);
        hoverCoroutine = StartCoroutine(ScaleCard(originalScale * scaleFactor, newPosition));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        hoverCoroutine = StartCoroutine(ScaleCard(originalScale, originalPosition));
    }

    private IEnumerator ScaleCard(Vector3 targetScale, Vector2 position)
    {        
        while (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, position, Time.deltaTime * scaleSpeed);
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
        }
    }
}
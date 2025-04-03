using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandVisualizer : MonoBehaviour
{
    [SerializeField] private int cardSpacing = 100;

    private float throwHeight = 2f;
    private float throwDuration = 0.5f;
    private AnimationCurve throwCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    private bool isAnimating = false;

    public void SetCardsPositions(List<GameObject> CardObjectsInHand)
    {
        var cardsToDisplay = CardObjectsInHand;
        if (cardsToDisplay.Count == 0)
            return;

        int positionX = 0;

        if (cardsToDisplay.Count % 2 == 0)
            positionX = -(cardsToDisplay.Count / 2 * cardSpacing - cardSpacing / 2);
        else
            positionX = -(cardsToDisplay.Count / 2 * cardSpacing);

        foreach (var card in cardsToDisplay)
        {
            card.SetActive(true);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionX, 0);
            positionX += cardSpacing;
        }
    }

    public void VisualizeCardRemoval(GameObject removedCard, List<GameObject> cardsInHand)
    {
        StartCoroutine(ThrowCardCoroutine(removedCard, cardsInHand));
    }

    public void VisualizeCardTaking(GameObject takenCard, List<GameObject> cardsInHand)
    {
        takenCard.SetActive(false);
        StartCoroutine(WaitAndSetCardsPositions(cardsInHand));
    }

    private IEnumerator WaitAndSetCardsPositions(List<GameObject> cardObjects)
    {
        while (isAnimating)
        {
            yield return null;
        }

        SetCardsPositions(cardObjects);
    }

    private IEnumerator ThrowCardCoroutine(GameObject cardObject, List<GameObject> cardObjects)
    {
        isAnimating = true;
        var canvasGroup = cardObject.GetComponent<CanvasGroup>();
        var rectTransform = cardObject.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + Vector2.up * throwHeight;

        float elapsedTime = 0f;
        while (elapsedTime < throwDuration)
        {
            float t = elapsedTime / throwDuration;
            float yOffset = throwCurve.Evaluate(t) * throwHeight;
            rectTransform.anchoredPosition = startPosition + Vector2.up * yOffset;
            canvasGroup.alpha = 1f - fadeCurve.Evaluate(t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPosition;
        canvasGroup.alpha = 0f;
        Destroy(cardObject);
        SetCardsPositions(cardObjects);
        isAnimating = false;
    }
}

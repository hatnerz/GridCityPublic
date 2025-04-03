using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Shared;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlace : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer buildingSpriteRenderer;
    [SerializeField] private SpriteRenderer flagSpriteRenderer;
    [SerializeField] private Sprite flagSprite;
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private Color cannotBuildHightlightColor = Color.yellow;
    [SerializeField] private Color canBuildHightlightColor = Color.green;

    private Vector3 _flagScale = new Vector3(0.18f, 0.18f);

    public IntCoordinates GridPosition { get; set; }
    public Building Building { get; set; }
    public BuildingData BuildingData { get; set; }
    public bool IsPlayerSpecific { get; set; } = false;
    public Color? PlacedPlayerColor { get; set; }

    public static event Action<BuildingPlace> OnMouseEnter;
    public static event Action<BuildingPlace> OnMouseExit;
    public static event Action<BuildingPlace> OnMouseClick;

    public void Awake()
    {
        if (highlightObject != null)
            highlightObject.SetActive(false);

        cannotBuildHightlightColor.a = 0.3f;
        canBuildHightlightColor.a = 0.3f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("invoked");
        OnMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMouseClick?.Invoke(this);
    }

    public void DisableHightlight()
    {
        if(highlightObject != null)
        {
            highlightObject.SetActive(false);
        }
    }

    public void SetHighlight(bool canBuild)
    {
        if (Building != null)
            return;

        if (highlightObject != null)
        {
            highlightObject.SetActive(true);

            var highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
            if (highlightRenderer != null)
            {
                highlightRenderer.color = canBuild ? canBuildHightlightColor : cannotBuildHightlightColor;
            }
        }
    }

    public void VisualizeBuilding()
    {
        buildingSpriteRenderer.sprite = BuildingData.BuildingSprite;
        buildingSpriteRenderer.transform.localScale = BuildingData.BuildingSpriteScale;
        buildingSpriteRenderer.transform.localPosition = BuildingData.BuildingSpriteOffset;
        buildingSpriteRenderer.sortingOrder = (GridPosition.X * 2 - GridPosition.Y * 2) + 3;

        if (IsPlayerSpecific)
        {
            flagSpriteRenderer.sprite = flagSprite;
            flagSpriteRenderer.sortingOrder = (GridPosition.X * 2 - GridPosition.Y * 2) + 4;
            flagSpriteRenderer.transform.localScale = _flagScale;
            flagSpriteRenderer.color = PlacedPlayerColor ?? Color.black;
        }
    }

    public IEnumerator AnimateBuilding(float duration = 0.5f, float g = 40f)
    {
        Transform buildingTransform = buildingSpriteRenderer.transform;

        Vector3 startPosition = new Vector3(buildingTransform.position.x, buildingTransform.position.y + 6f, buildingTransform.position.z);

        Vector3 endPosition = buildingTransform.position;

        buildingTransform.position = startPosition;

        float elapsedTime = 0f;
        float velocity = 2f;
        PlayImpactSound();

        while (elapsedTime < duration)
        {
            velocity += g * Time.deltaTime;
            float deltaY = velocity * Time.deltaTime;
            buildingTransform.position = new Vector3(
                startPosition.x,
                Mathf.Max(buildingTransform.position.y - deltaY, endPosition.y),
                startPosition.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buildingTransform.position = endPosition;

        PlayParticles();
    }

    private void PlayParticles()
    {
        var particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    private void PlayImpactSound()
    {
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

}

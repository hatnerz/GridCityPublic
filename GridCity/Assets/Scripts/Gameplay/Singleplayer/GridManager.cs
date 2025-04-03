using Assets.Scripts.Core;
using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using GridCity.GameLogic.Shared;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour, IGridState
{
    [SerializeField] private GridVisualizer gridVisualizer;

    private bool isGridEmpty { get; set; }

    public int SizeX { get { return GridElements.GetLength(0); } }
    public int SizeY { get { return GridElements.GetLength(1); } }

    public CellElement[,] GridElements { get; private set; }
    public BuildingPlace[,] BuildingPlaces { get; private set; }

    public event Action<BuildingPlace> BuildingPlaced;
    public event Action<BuildingPlace> ValidBuildingPlaceClicked;

    public void InitializeGrid(Vector2Int gridSize, bool isPlayerSpecific)
    {
        if (gridVisualizer == null)
        {
            throw new MissingReferenceException("GridVisualizer is not set in GridManager");
        }

        gridVisualizer.VisualizeGrid(gridSize);
        BuildingPlaces = gridVisualizer.CreateAllBuildingPlaces(gridSize, isPlayerSpecific);
        GridElements = new CellElement[gridSize.x, gridSize.y];
        isGridEmpty = true;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridElements[x, y] = new Ground("Land", GroundType.Land);
            }
        }
    }

    public void PlaceBuilding(Building buildingToBuild, Vector2Int buildingPlacePosition, Color? playerColor = null)
    {
        var buildingPlace = BuildingPlaces[buildingPlacePosition.x, buildingPlacePosition.y];
        buildingToBuild.GridPosition = new IntCoordinates(buildingPlacePosition.x, buildingPlacePosition.y);

        var buildingData = ResourceManager.Instance.BuildingDataDictionary[buildingToBuild.BuildingType];

        buildingPlace.Building = buildingToBuild;
        buildingPlace.BuildingData = buildingData;
        buildingPlace.PlacedPlayerColor = playerColor;
        buildingPlace.VisualizeBuilding();
        GridElements[buildingPlace.GridPosition.X, buildingPlace.GridPosition.Y] = buildingPlace.Building;

        buildingPlace.DisableHightlight();

        BuildingPlaced?.Invoke(buildingPlace);

        isGridEmpty = false; // TODO: Refactor

        StartCoroutine(buildingPlace.AnimateBuilding());
    }

    private void OnEnable()
    {
        BuildingPlace.OnMouseEnter += HandleBuildingPlaceEnter;
        BuildingPlace.OnMouseExit += HandleBuildingPlaceExit;
        BuildingPlace.OnMouseClick += HandleBuildingPlaceClick;
    }

    private void OnDisable()
    {
        BuildingPlace.OnMouseEnter -= HandleBuildingPlaceEnter;
        BuildingPlace.OnMouseExit -= HandleBuildingPlaceExit;
        BuildingPlace.OnMouseClick -= HandleBuildingPlaceClick;
    }

    private void HandleBuildingPlaceEnter(BuildingPlace buildingPlace)
    {
        Debug.Log("Enter budilding place");
        buildingPlace.SetHighlight(IsAllowedToBuild(buildingPlace) && GameState.IsPlayersTurn);
    }

    private void HandleBuildingPlaceExit(BuildingPlace buildingPlace)
    {
        buildingPlace.DisableHightlight();
    }

    private void HandleBuildingPlaceClick(BuildingPlace buildingPlace)
    {
        if (!IsAllowedToBuild(buildingPlace) || !GameState.IsPlayersTurn)
            return;

        ValidBuildingPlaceClicked?.Invoke(buildingPlace);
    }

    private bool IsAllowedToBuild(BuildingPlace buildingPlace)
    {
        if (GridElements[buildingPlace.GridPosition.X, buildingPlace.GridPosition.Y] is not Ground)
            return false;

        var hasNeighbors = GridElementsHelper.GetAdjacentBuildings(buildingPlace.GridPosition, this).Count > 0;

        if (!isGridEmpty && !hasNeighbors)
            return false;

        return true;
    }
}


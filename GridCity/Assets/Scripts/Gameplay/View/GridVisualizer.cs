using GridCity.GameLogic.Shared;
using System.Collections.Generic;
using UnityEngine;


public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private float cellSizeX = 3.5f;
    [SerializeField] private float cellSizeY = 2f;
    [SerializeField] private float buildingPlaceOffsetX = 0;
    [SerializeField] private float buildingPlaceOffsetY = 1.5f;
    [SerializeField] private GameObject buildingPlacePrefab;
    [SerializeField] private Sprite[] groundSprites;

    private Vector2Int drawGridSize;

    private const string emptyGroundSpriteName = "ground_empty";

    private const string leftCornerRoadSpriteName = "ground_left_corner_2";
    private const string rightCornerRoadSpriteName = "ground_right_corner_2";
    private const string topCornerRoadSpriteName = "ground_top_corner_2";
    private const string bottomCornerRoadSpriteName = "ground_bottom_corner_2";

    private const string leftCrossRoadSpriteName = "ground_left_cross";
    private const string rightCrossRoadSpriteName = "ground_right_cross";
    private const string topCrossRoadSpriteName = "ground_top_cross";
    private const string bottomCrossRoadSpriteName = "ground_bottom_cross";
    private const string fullCrossRoadSpriteName = "ground_cross";

    private const string straightXRoadSpriteName = "ground_straight_x";
    private const string straightYRoadSpriteName = "ground_straight_y";

    private Dictionary<string, Sprite> spriteDict;

    void Awake()
    {
        InitializeSpriteDict();
        Debug.Log($"Grid sprites initialized: {spriteDict.Count}");
    }

    public void VisualizeGrid(Vector2Int gridSize)
    {
        drawGridSize = CalculateActualIsometricGridSize(gridSize);
        DrawGround();
    }

    public BuildingPlace[,] CreateAllBuildingPlaces(Vector2Int gridSize, bool isPlayerSpecific)
    {
        var buildindPlaces = new BuildingPlace[gridSize.x, gridSize.y];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                buildindPlaces[i, j] = CreateBuildingPlace(new IntCoordinates(i, j), isPlayerSpecific);
            }
        }

        return buildindPlaces;
    }


    private void InitializeSpriteDict()
    {
        spriteDict = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in groundSprites)
        {
            spriteDict[sprite.name] = sprite;
        }
    }

    private void DrawGround()
    {
        DrawOutline();
        DrawOuterRoads();
        DrawInnerRoads();
    }

    private void DrawOutline()
    {
        var groundSprite = spriteDict[emptyGroundSpriteName];
        DrawGroundTile(new Vector2Int(0, 0), groundSprite, 0);
        DrawGroundTile(new Vector2Int(0, drawGridSize.y - 1), groundSprite, -drawGridSize.y + 1);
        DrawGroundTile(new Vector2Int(drawGridSize.x - 1, 0), groundSprite, drawGridSize.x - 1);
        DrawGroundTile(new Vector2Int(drawGridSize.x - 1, drawGridSize.y - 1), groundSprite, drawGridSize.x - drawGridSize.y);

        for(int i = 1; i < drawGridSize.y - 1; i++)
        {
            DrawGroundTile(new Vector2Int(0, i), groundSprite, -i);
            DrawGroundTile(new Vector2Int(drawGridSize.x - 1, i), groundSprite, drawGridSize.x - i - 1);
        }

        for (int i = 1; i < drawGridSize.x - 1; i++)
        {
            DrawGroundTile(new Vector2Int(i, 0), groundSprite, i);
            DrawGroundTile(new Vector2Int(i, drawGridSize.y - 1), groundSprite, -drawGridSize.y + i + 1);
        }
    }
    
    private void DrawOuterRoads()
    {
        var leftCornerRoadSprite = spriteDict[leftCornerRoadSpriteName];
        var rightCornerRoadSprite = spriteDict[rightCornerRoadSpriteName];
        var topCornerRoadSprite = spriteDict[topCornerRoadSpriteName];
        var bottomCornerRoadSprite = spriteDict[bottomCornerRoadSpriteName];

        var leftCrossRoadSprite = spriteDict[leftCrossRoadSpriteName];
        var rightCrossRoadSprite = spriteDict[rightCrossRoadSpriteName];
        var topCrossRoadSprite = spriteDict[topCrossRoadSpriteName];
        var bottomCrossRoadSprite = spriteDict[bottomCrossRoadSpriteName];

        var straightXRoadSprite = spriteDict[straightXRoadSpriteName];
        var straightYRoadSprite = spriteDict[straightYRoadSpriteName];

        DrawGroundTile(new Vector2Int(1, 1), leftCornerRoadSprite, 0);
        DrawGroundTile(new Vector2Int(1, drawGridSize.y - 2), topCornerRoadSprite, -drawGridSize.y + 3);
        DrawGroundTile(new Vector2Int(drawGridSize.x - 2, 1), bottomCornerRoadSprite, drawGridSize.x - 3);
        DrawGroundTile(new Vector2Int(drawGridSize.x - 2, drawGridSize.y - 2), rightCornerRoadSprite, drawGridSize.x - drawGridSize.y);

        for (int i = 2; i < drawGridSize.y - 2; i++)
        {
            if (i % 2 == 0)
            {
                DrawGroundTile(new Vector2Int(1, i), straightYRoadSprite, -i + 1);
                DrawGroundTile(new Vector2Int(drawGridSize.x - 2, i), straightYRoadSprite, drawGridSize.x - i - 2);
            }
            else
            {
                DrawGroundTile(new Vector2Int(1, i), leftCrossRoadSprite, -i + 1);
                DrawGroundTile(new Vector2Int(drawGridSize.x - 2, i), rightCrossRoadSprite, drawGridSize.x - i - 2);
            }
        }

        for (int i = 2; i < drawGridSize.x - 2; i++)
        {
            if (i % 2 == 0)
            {
                DrawGroundTile(new Vector2Int(i, 1), straightXRoadSprite, i - 1);
                DrawGroundTile(new Vector2Int(i, drawGridSize.y - 2), straightXRoadSprite, -drawGridSize.y + i + 2);
            }
            else
            {
                DrawGroundTile(new Vector2Int(i, 1), bottomCrossRoadSprite, i - 1);
                DrawGroundTile(new Vector2Int(i, drawGridSize.y - 2), topCrossRoadSprite, -drawGridSize.y + i + 2);
            }
        }
    }

    private void DrawInnerRoads()
    {
        var straightXRoadSprite = spriteDict[straightXRoadSpriteName];
        var straightYRoadSprite = spriteDict[straightYRoadSpriteName];
        var crossRoadSprite = spriteDict[fullCrossRoadSpriteName];
        var emptyGroundSprite = spriteDict[emptyGroundSpriteName];

        for (int i = 2; i < drawGridSize.x - 2; i++)
        {
            for (int j = 2; j < drawGridSize.y - 2; j++)
            {
                if(i % 2 == 0)
                {
                    if(j % 2 == 0)
                    {
                        DrawGroundTile(new Vector2Int(i, j), emptyGroundSprite, i - j);
                    }
                    else
                    {
                        DrawGroundTile(new Vector2Int(i, j), straightXRoadSprite, i - j);
                    }
                }
                else
                {
                    if(j % 2 == 0)
                    {
                        DrawGroundTile(new Vector2Int(i, j), straightYRoadSprite, i - j);
                    }
                    else
                    {
                        DrawGroundTile(new Vector2Int(i, j), crossRoadSprite, i - j);
                    }
                }
            }
        }
    }

    private void DrawGroundTile(Vector2Int isometricPosition, Sprite sprite, int order)
    {
        var spritePosition = GetActualPositionByIsometricPosition(isometricPosition);

        GameObject tileObject = new GameObject($"Ground_Tile_{isometricPosition.x}_{isometricPosition.y}");
        tileObject.transform.SetParent(transform);
        tileObject.transform.localPosition = spritePosition;

        SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = order;
    }

    private Vector2Int CalculateActualIsometricGridSize(Vector2Int gridSize)
    {
        var x = gridSize.x + gridSize.x + 1 + 2;
        var y = gridSize.y + gridSize.y + 1 + 2;

        return new Vector2Int(x, y);
    }

    private BuildingPlace CreateBuildingPlace(IntCoordinates gridPosition, bool isPlayerSpecific)
    {
        var actualPosition = GetActualPositionByGridBuildingPosition(gridPosition);
        var buildingPlaceCreatedObject = Instantiate(buildingPlacePrefab, actualPosition, Quaternion.identity, transform);
        buildingPlaceCreatedObject.name = $"BuildingPlace_grid_{gridPosition.X}_{gridPosition.Y}";
        var buildingPlace = buildingPlaceCreatedObject.GetComponent<BuildingPlace>();
        Debug.Log(gridPosition);
        buildingPlace.GridPosition = gridPosition;
        buildingPlace.IsPlayerSpecific = isPlayerSpecific;
        return buildingPlace;
    }

    private Vector2 GetActualPositionByIsometricPosition(Vector2Int isometricPosition)
    {
        return new Vector2(
            (isometricPosition.x + isometricPosition.y) * cellSizeX,
            (isometricPosition.y - isometricPosition.x) * cellSizeY);
    }

    private Vector2 GetActualPositionByGridBuildingPosition(IntCoordinates gridPosition)
    {
        Debug.Log($"Position: {gridPosition.X} {gridPosition.Y}");
        return new Vector2(
            (gridPosition.X + gridPosition.Y) * cellSizeX * 2 + cellSizeX * 4 + buildingPlaceOffsetX,
            (gridPosition.Y - gridPosition.X) * cellSizeY * 2 + buildingPlaceOffsetY);
    }
}
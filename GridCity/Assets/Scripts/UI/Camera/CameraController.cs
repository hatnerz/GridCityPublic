using Assets.Scripts.Core;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float panSpeed = 50f;
    [SerializeField] private float minPanSpeed = 6.67f;
    [SerializeField] private float maxPanSpeed = 50f;
    [SerializeField] private float panBorderThickness = 100f;
    [SerializeField] private float cellRatioX = 4.4f; 
    [SerializeField] private float cellRatioY = 2.54f;

    private Camera cam;
    private LevelData levelData;
    private Vector2 panMin;
    private Vector2 panMax;

    private void Start()
    {
        cam = GetComponent<Camera>();
        InitiateBorder();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll);

        PanCamera();
    }

    private void InitiateBorder()
    {
        var currentLevel = GameState.CurrentLevelNumber;
        levelData = ResourceManager.Instance.LevelDataDictionary[currentLevel];

        int groundSqares = levelData.GridSize.x * 2 + 3;

        float groundWidth = Mathf.Sqrt(Mathf.Pow(groundSqares, 2) + Mathf.Pow(groundSqares, 2));


        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        panMin.x = (((groundWidth * cellRatioX) / groundSqares) * 0.5f);
        panMax.x = ((groundWidth * cellRatioX) / groundSqares) * (groundSqares - 0.5f);
        panMin.y = -((groundWidth * cellRatioY) / groundSqares) * (groundSqares / 2 - 0.5f);
        panMax.y = ((groundWidth * cellRatioY) / groundSqares) * (groundSqares / 2 - 0.5f);

        transform.position = new Vector3((panMin.x + panMax.x) / 2, 0, transform.position.z);
    }

    private void ZoomCamera(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment * zoomSpeed, minZoom, maxZoom);
        panSpeed = Mathf.Clamp(panSpeed - increment * 10, minPanSpeed, maxPanSpeed);
    }

    private void PanCamera()
    {
        Vector3 pos = transform.position;

        // Рух камери на основі вводу
        if (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.GetKey("w"))
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness || Input.GetKey("s"))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness || Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness || Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, panMin.x, panMax.x);
        pos.y = Mathf.Clamp(pos.y, panMin.y, panMax.y);

        transform.position = pos;
    }
}
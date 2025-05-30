using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookScript : MonoBehaviour, BuildInputSystem.IBuildActions
{
    [SerializeField] private Transform rayOrigin;

    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private float rayLength;

    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject ramp;

    [SerializeField] private float timeBetweenBuilds;
    private float nextBuildTime;

    private GameObject _currentObject;
    private GameObject _currentPreviewObject;

    private bool _hit;
    [SerializeField] private Vector3 rayHitPoint;
    [SerializeField] private Vector3 calcedObjPos;
    [SerializeField] private Vector3 newCalcedPos;
    [SerializeField] private int clampedRotation;
    [SerializeField] private bool buildingBool;
    private readonly HashSet<string> builtObjects = new();

    private BuildInputSystem _buildInputSystem;
    private NavMeshUpdater _navMeshUpdater;

    private void Awake()
    {
        // Erstelle eine Instanz des BuildInputSystem
        _buildInputSystem = new BuildInputSystem();
        _currentObject = wall;
        _navMeshUpdater = GameObject.FindGameObjectWithTag("Ground").GetComponent<NavMeshUpdater>();
    }

    private void OnEnable()
    {
        // Aktiviere die Input-Aktionen und registriere die Callbacks
        _buildInputSystem.Build.SetCallbacks(this);
        _buildInputSystem.Build.Enable();
        fireAction.action.started += OnBuild;
        fireAction.action.canceled += OnBuild;
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        // Deaktiviere die Input-Aktionen und entferne die Callbacks
        _buildInputSystem.Build.Disable();
        _buildInputSystem.Build.SetCallbacks(null);
        fireAction.action.started -= OnBuild;
        fireAction.action.canceled -= OnBuild;
        fireAction.action.Disable();
    }

    // Callback f�r die "Wall"-Aktion
    public void OnWall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentObject = wall;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
            calcedObjPos = Vector3.zero;
        }
    }

    // Callback f�r die "Floor"-Aktion
    public void OnFloor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentObject = floor;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
            calcedObjPos = Vector3.zero;
        }
    }

    // Callback f�r die "Ramp"-Aktion
    public void OnRamp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentObject = ramp;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
            calcedObjPos = Vector3.zero;
        }
    }

    private void Update()
    {
        transform.rotation = playerCameraRoot.transform.rotation;
        this._hit = Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit, rayLength);
        if (_currentPreviewObject is null)
        {
            _currentPreviewObject = Instantiate(_currentObject);
            _currentPreviewObject.GetComponentInChildren<BuildingHealth>().SetBookScript(this);
            _currentPreviewObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            var colliders = _currentPreviewObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            UpdatePreviewObject();
        }
        rayHitPoint = hit.point;

        if (RoundToNearestEven(rayHitPoint) != calcedObjPos || clampedRotation != ClampToNearest90(playerCameraRoot.transform.eulerAngles.y))
        {
            calcedObjPos = RoundToNearestEven(rayHitPoint);
            clampedRotation = ClampToNearest90(playerCameraRoot.transform.eulerAngles.y);
            UpdatePreviewObject();
        }

        if (buildingBool)
        {
            Build();
        }
    }

    private void UpdatePreviewObject()
    {
        if (!_hit)
        {
            _currentPreviewObject.transform.position = new Vector3(0, -1000, 0);
            return;
        }
        var newPos = calcedObjPos;
        if (CheckIfObjectAlreadyExists(newPos))
        {
            if (_currentPreviewObject.CompareTag("Floor") || _currentPreviewObject.CompareTag("Ramp"))
            {
                switch (ClampToNearest90(playerCameraRoot.transform.eulerAngles.y))
                {
                    case 0:
                        newPos.z += 4;
                        break;
                    case 90:
                        newPos.x += 4;
                        break;
                    case 180:
                        newPos.z -= 4;
                        break;
                    case 270:
                        newPos.x -= 4;
                        break;
                }
            }
            if (_currentPreviewObject.CompareTag("Ramp"))
            {
                newPos.y += 4;
            }
        }
        // Setze die Position des Vorschauobjekts
        newCalcedPos = newPos;
        _currentPreviewObject.transform.position = newPos;
        _currentPreviewObject.transform.rotation = Quaternion.Euler(0, ClampToNearest90(playerCameraRoot.transform.eulerAngles.y), 0);
    }

    private Vector3 RoundToNearestEven(Vector3 value)
    {
        if (_currentObject == floor)
        {
            value = new Vector3(value.x, value.y + 2, value.z);
        }
        return new Vector3(RoundToNearestMultiple(value.x, 4), RoundToNearestMultiple(value.y, 4), RoundToNearestMultiple(value.z, 4));
    }

    private bool CheckIfObjectAlreadyExists(Vector3 position)
    {
        var objectString = _currentPreviewObject.tag + (_currentPreviewObject.tag.Equals("Wall") ? _currentPreviewObject.transform.eulerAngles.y : "") + position;
        return builtObjects.Contains(objectString);
    }

    private static int RoundToNearestMultiple(float value, int multiple)
    {
        return Mathf.RoundToInt(value / multiple) * multiple;
    }

    private static int ClampToNearest90(float angle)
    {
        return (Mathf.RoundToInt(angle / 90f) * 90) % 360;
    }

    private void OnBuild(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            buildingBool = true;
        }
        else if (context.canceled)
        {
            buildingBool = false;
        }
    }

    private void Build()
    {
        if (Time.time < nextBuildTime)
            return;
        nextBuildTime = Time.time + timeBetweenBuilds;
        var newObject = _currentPreviewObject;
        if (!newObject && !_hit)
        {
            return;
        }
        var objectString = GenerateObjectName(newObject);
        if (builtObjects.Contains(objectString))
        {
            return;
        }
        builtObjects.Add(objectString);
        // Aktiviere den MeshCollider nur f�r das aktuelle Objekt
        var meshCollider = newObject.GetComponentInChildren<MeshCollider>();
        if (meshCollider)
        {
            meshCollider.enabled = true;
        }

        // Instanziere neue Materialien, um �nderungen nur auf das aktuelle Objekt anzuwenden
        newObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
        var renderers = newObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.materials = renderer.materials.Select(material => new Material(material)).ToArray();
            foreach (var material in renderer.materials)
            {
                material.SetFloat("_Mode", 0); // Opaque Mode
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1; // Standard-Render-Queue f�r Opaque
            }
        }
        _navMeshUpdater.UpdateNavMesh();
        _currentPreviewObject = null;
    }

    private string GenerateObjectName(GameObject obj)
    {
        return obj.tag + (obj.tag.Equals("Wall") ? obj.transform.eulerAngles.y : "") + obj.transform.position;
    }

    public void CleanUp()
    {
        Destroy(_currentPreviewObject);
        _currentPreviewObject = null;
    }
    public void RemoveBuilding(GameObject obj)
    {
        builtObjects.Remove(GenerateObjectName(obj));
        _navMeshUpdater.UpdateNavMesh();
    }
}

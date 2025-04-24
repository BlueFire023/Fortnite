using StarterAssets;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookScript : MonoBehaviour, BuildInputSystem.IBuildActions
{
    private BuildInputSystem _input;
    public Transform RayOrigin;

    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private float rayLength;

    [SerializeField] private GameObject Wall;
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject Ramp;

    private GameObject _currentObject;
    private GameObject _currentPreviewObject;

    private RaycastHit _raycastHit;

    private BuildInputSystem _buildInputSystem;

    private void Awake()
    {
        // Erstelle eine Instanz des BuildInputSystem
        _buildInputSystem = new BuildInputSystem();
        _currentObject = Wall;
    }

    private void OnEnable()
    {
        // Aktiviere die Input-Aktionen und registriere die Callbacks
        _buildInputSystem.Build.SetCallbacks(this);
        _buildInputSystem.Build.Enable();
        fireAction.action.started += OnBuild;
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        // Deaktiviere die Input-Aktionen und entferne die Callbacks
        _buildInputSystem.Build.Disable();
        _buildInputSystem.Build.SetCallbacks(null);
        fireAction.action.started -= OnBuild;
        fireAction.action.Disable();
    }

    // Callback für die "Wall"-Aktion
    public void OnWall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Wall action triggered!");
            _currentObject = Wall;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
        }
    }

    // Callback für die "Floor"-Aktion
    public void OnFloor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Floor action triggered!");
            _currentObject = Floor;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
        }
    }

    // Callback für die "Ramp"-Aktion
    public void OnRamp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ramp action triggered!");
            _currentObject = Ramp;
            Destroy(_currentPreviewObject);
            _currentPreviewObject = null;
        }
    }

    private void Update()
    {
        transform.rotation = playerCameraRoot.transform.rotation;
        Physics.Raycast(RayOrigin.position, RayOrigin.forward, out _raycastHit, rayLength);
        Debug.DrawLine(RayOrigin.position, _raycastHit.point, Color.red);
        if (_currentPreviewObject is null)
        {
            _currentPreviewObject = Instantiate(_currentObject);

            var colliders = _currentPreviewObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }

        // Setze die Position des Vorschauobjekts
        _currentPreviewObject.transform.position = RoundToNearestEven(_raycastHit.point);
        _currentPreviewObject.transform.rotation = Quaternion.Euler(0, ClampToNearest90(playerCameraRoot.transform.eulerAngles.y), 0);
    }

    private Vector3 RoundToNearestEven(Vector3 value)
    {
        // Runde auf die nächste gerade Zahl
        return new Vector3(RoundToNearestMultipleOfThree(value.x), RoundToNearestMultipleOfThree(value.y), RoundToNearestMultipleOfThree(value.z));
    }

    private float RoundToNearestMultipleOfThree(float value)
    {
        // Runde auf die nächste ganze Zahl
        int rounded = Mathf.RoundToInt(value);

        // Stelle sicher, dass die Zahl durch 3 teilbar ist
        int remainder = rounded % 4;
        if (remainder == 0)
        {
            return rounded; // Bereits durch 3 teilbar
        }

        // Runde auf die nächste Zahl, die durch 3 teilbar ist
        return rounded + (4 - remainder);
    }

    private float ClampToNearest90(float angle)
    {
        return Mathf.Round(angle / 90f) * 90f;
    }
    private void OnBuild(InputAction.CallbackContext context)
    {
        var newObject = _currentPreviewObject;

        // Aktiviere den MeshCollider nur für das aktuelle Objekt
        var meshCollider = newObject.GetComponentInChildren<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }

        // Instanziere neue Materialien, um Änderungen nur auf das aktuelle Objekt anzuwenden
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
                material.renderQueue = -1; // Standard-Render-Queue für Opaque
            }
        }

        // Setze das Vorschauobjekt zurück
        _currentPreviewObject = null;
    }

}

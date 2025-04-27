using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public bool isShooting;

    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float timeBetweenShots = 0.2f;
    public float shootForce = 100f;

    private float nextShootTime = 0f;
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference bombAction;
    [SerializeField] private GameObject playerCameraRoot;
    [SerializeField] private GameObject bomb;

    private void Update()
    {
        transform.rotation = playerCameraRoot.transform.rotation; // Setzt die Rotation des Spielers auf die der Kamera
        if (isShooting)
            Shoot();
    }

    private void OnEnable()
    {
        fireAction.action.started += OnFire;
        fireAction.action.canceled += OnFire;
        fireAction.action.Enable();
        bombAction.action.performed += OnBomb;
        bombAction.action.Enable();
    }

    private void OnDisable()
    {
        fireAction.action.started -= OnFire;
        fireAction.action.canceled -= OnFire;
        fireAction.action.Disable();
        bombAction.action.performed -= OnBomb;
        bombAction.action.Disable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
            isShooting = true;
        else if (context.canceled)
            isShooting = false;
    }

    public void OnBomb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var bombInstance = Instantiate(bomb, shootPoint.position, Quaternion.Euler(
                bomb.transform.rotation.eulerAngles.x,
                shootPoint.rotation.eulerAngles.y,
                shootPoint.rotation.eulerAngles.z
            ));
            var rb = bombInstance.GetComponent<Rigidbody>();
            rb.AddForce(shootPoint.forward * shootForce / 5, ForceMode.Impulse);
        }
    }

    private void Shoot()
    {
        if (Time.time < nextShootTime)
            return;

        var projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(
            projectilePrefab.transform.rotation.eulerAngles.x,
            shootPoint.rotation.eulerAngles.y,
            shootPoint.rotation.eulerAngles.z
        ));
        var rb = projectile.GetComponent<Rigidbody>();

        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
        nextShootTime = Time.time + timeBetweenShots;
    }
}

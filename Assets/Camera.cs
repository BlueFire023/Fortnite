using NUnit.Framework.Constraints;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float minYAngle = -45.0f;
    [SerializeField] private float maxYAngle = 45.0f;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private LayerMask cameraLayerMask;
    public bool isHit = false;

    private float currentYRotation = 0.0f;
    public void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            var mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Rotate around Y axis
            player.transform.Rotate(0, mouseX, 0);

            // Rotate around Z axis with clamping
            currentYRotation -= mouseY;
            currentYRotation = Mathf.Clamp(currentYRotation, minYAngle, maxYAngle);
            transform.localEulerAngles = new Vector3(currentYRotation, transform.localEulerAngles.y, 0);
        }

        if (Physics.Raycast(transform.position, -transform.forward, out var hit, 5f, cameraLayerMask))
        {
            playerCamera.transform.position = hit.point + transform.forward * 0.5f;
            isHit = true;
        }
        else
        {
            playerCamera.transform.position = transform.position - transform.forward * 5f;
            isHit = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isHit) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawLine(transform.position, playerCamera.transform.position);
    }
}

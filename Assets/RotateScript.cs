using UnityEngine;

public class RotateScript : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0,0.1f,0));
    }
}

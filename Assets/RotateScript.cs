using UnityEngine;

public class RotateScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0.1f,0));
    }
}

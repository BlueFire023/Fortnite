using UnityEngine;

public class TargetUpdate : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            targetObject.transform.position = new Vector3(transform.position.x,
                transform.position.y + targetObject.transform.localScale.y / 2f,
                transform.position.z);
        }
    }
}

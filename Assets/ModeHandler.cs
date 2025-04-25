using UnityEngine;

public class ModeHandler : MonoBehaviour
{
    [SerializeField] private GameObject gunObject;
    [SerializeField] private GameObject bookObject;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gunObject.SetActive(!gunObject.activeSelf);
            bookObject.SetActive(!bookObject.activeSelf);

            gunObject.GetComponent<GunScript>().enabled = gunObject.activeSelf;
            bookObject.GetComponent<BookScript>().enabled = bookObject.activeSelf;
            bookObject.GetComponent<BookScript>().CleanUp();
        }
    }
}

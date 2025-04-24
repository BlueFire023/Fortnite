using UnityEngine;

public class ModeHandler : MonoBehaviour
{
    [SerializeField] private GameObject GunObject;
    [SerializeField] private GameObject BookObject;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GunObject.SetActive(!GunObject.activeSelf);
            BookObject.SetActive(!BookObject.activeSelf);

            GunObject.GetComponent<GunScript>().enabled = GunObject.activeSelf;
            BookObject.GetComponent<BookScript>().enabled = BookObject.activeSelf;
        }
    }
}

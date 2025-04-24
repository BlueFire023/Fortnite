//using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionController : MonoBehaviour
{

    public Button btServer;
    public Button btHost;
    public Button btClient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //btServer.onClick.AddListener(() => { NetworkManager.Singleton.StartServer(); });
        //btHost.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });    
        //btClient.onClick.AddListener(() => { NetworkManager.Singleton.StartClient();  });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

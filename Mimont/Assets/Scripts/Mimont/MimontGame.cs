using Mimont.Netcode;
using UnityEngine;

namespace Mimont {
public class MimontGame : MonoBehaviour {
    private MimontServer server;
    private MimontClient client;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private RingManager ringManager;
    
    private bool isServer;

    private void Awake() {
        inputHandler.gameObject.SetActive(false);
        targetSpawner.gameObject.SetActive(false);
        ringManager.gameObject.SetActive(false);
    }

    private void StartGame(bool isHost) {
        isServer = isHost;
        // If starting as server, awaken server
        if (isServer) {
            server = new MimontServer();
            server.Start();
        }

        // Awaken client
        client = new MimontClient();
        client.Connect();
    }

    private void AwakenGameObjects() {
        // Link all client callbacks
        
        // Wake up objects
        inputHandler.gameObject.SetActive(true);
        targetSpawner.gameObject.SetActive(true);
        ringManager.gameObject.SetActive(true);
    }
}
}

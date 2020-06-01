using UnityEngine;

namespace Mimont {
public class RingManager : MonoBehaviour {
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Ring ringPrefab;

    private Ring playerRing;
    private Ring opponentRing;
    
    private void Start() {
        if (!inputHandler) {
            Debug.LogWarning("Input handler not set, using FindObjectOfType. Please set an input handler!");
            FindObjectOfType<InputHandler>();
        }

        // Spawn rings
        playerRing = Instantiate(ringPrefab, transform, false);
        opponentRing = Instantiate(ringPrefab, transform, false);

        inputHandler.WorldClickPerformed += playerRing.Activate;
        inputHandler.HoldReleased += playerRing.Release;
    }

    private void OnDisable() {
        inputHandler.WorldClickPerformed -= playerRing.Activate;
        inputHandler.HoldReleased -= playerRing.Release;
    }
}
}

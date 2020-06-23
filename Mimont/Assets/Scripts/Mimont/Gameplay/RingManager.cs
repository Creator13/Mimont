using System;
using UnityEngine;

namespace Mimont.Gameplay {
public class RingManager : MonoBehaviour {
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    [SerializeField] private SpherePool spherePool;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Ring ringPrefab;

    private Ring playerRing;
    private Ring opponentRing;

    public event Action<Vector3> RingCreated;
    public event Action RingReleased;

    private void Start() {
        if (!inputHandler) {
            Debug.LogWarning("Input handler not set, using FindObjectOfType. Please set an input handler!");
            FindObjectOfType<InputHandler>();
        }

        // Spawn rings
        playerRing = Instantiate(ringPrefab, transform);
        opponentRing = Instantiate(ringPrefab, transform);

        playerRing.IsPlayer = true;
        opponentRing.IsPlayer = false;
        playerRing.SpherePool = spherePool;
        opponentRing.SpherePool = spherePool;

        // var mpb = new MaterialPropertyBlock();
        // mpb.SetColor(BaseColor, new Color32(155, 100, 80, 134));
        // opponentRing.gameObject.GetComponent<Renderer>().SetPropertyBlock(mpb);

        inputHandler.WorldClickPerformed += StartPlayerRing;
        inputHandler.HoldReleased += ReleasePlayerRing;
    }

    private void OnDisable() {
        inputHandler.WorldClickPerformed -= StartPlayerRing;
        inputHandler.HoldReleased -= ReleasePlayerRing;
    }

    private void StartPlayerRing(Vector3 position) {
        playerRing.Activate(position);
        RingCreated?.Invoke(position);
    }

    private void ReleasePlayerRing() {
        playerRing.Release();
        RingReleased?.Invoke();
    }

    public void StartOpponentRing(Vector3 position) {
        opponentRing.Activate(position);
    }

    public void ReleaseOpponentRing() {
        opponentRing.Release();
    }
}
}

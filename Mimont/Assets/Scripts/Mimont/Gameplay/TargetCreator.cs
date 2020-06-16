using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mimont.Gameplay {
public class TargetCreator : MonoBehaviour {
    [SerializeField] private new Camera camera;

    [Space(10)] [SerializeField] private TargetTierSettings tierSettings;
    [SerializeField] private float spawnRateMultiplier = 1;
    [SerializeField] private Target targetPrefab;
    [SerializeField] private float spawnRate = 5;

    [Space(10)] [Tooltip("In cm")] [SerializeField] private int playerHeight = 180;
    [Tooltip("In cm")] [SerializeField] private int screenHeight = 239;

    private bool paused = true;
    private float timeSinceLastSpawn;

    private float CameraWidth => camera.orthographicSize * 2 * camera.aspect;
    private float EdgeClearance => MaxTargetRadius / CameraWidth;
    private float MaxTargetRadius => targetPrefab ? targetPrefab.maxRadius : 0;

    public bool Paused {
        get => paused;
        set {
            paused = value;
            if (value) timeSinceLastSpawn = spawnRate;
        }
    }

    public event Action<Vector3, int> TargetCreated;

    public void StartSpawning(int delay) {
        StartCoroutine(Countdown(delay, () => Paused = false));
    }

    private static IEnumerator Countdown(int seconds, Action callback) {
        while (seconds > 0) {
            seconds--;
            yield return new WaitForSeconds(1);
        }

        callback();
    }

    private void Update() {
        if (Paused) return;

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > spawnRate) {
            CreateTarget();
            timeSinceLastSpawn = 0;
        }
    }

    private void CreateTarget() {
        var tier = Random.Range(0, tierSettings.tiers.Count);

        var spawnRect = GetSpawnRect();
        var viewportPos = new Vector2(
            Random.Range(spawnRect.xMin, spawnRect.xMax),
            Random.Range(spawnRect.yMin, spawnRect.yMax)
        );
        var pos = ViewportToWorldPoint(viewportPos);

        TargetCreated?.Invoke(pos, tier);
    }

    private Rect GetSpawnRect() {
        /* Viewport rect: (w,h)
         * (0,1)-----(1,1)
         *  |           |
         *  |           |
         * (0,0)-----(1,0)
         */

        var playerHeightRelative = (float) playerHeight / screenHeight;
        var edgeClearance = EdgeClearance;

        return new Rect(edgeClearance, playerHeightRelative * .6f, 1 - edgeClearance * 2, playerHeightRelative * .45f);
    }

    private void OnDrawGizmos() {
        var z = transform.position.z;

        // draw viewport rect
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(ViewportToWorldPoint(0, 0, z), ViewportToWorldPoint(0, 1, z));
        Gizmos.DrawLine(ViewportToWorldPoint(0, 1, z), ViewportToWorldPoint(1, 1, z));
        Gizmos.DrawLine(ViewportToWorldPoint(1, 1, z), ViewportToWorldPoint(1, 0, z));
        Gizmos.DrawLine(ViewportToWorldPoint(1, 0, z), ViewportToWorldPoint(0, 0, z));

        // draw spawn rect
        var rect = GetSpawnRect();
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ViewportToWorldPoint(rect.xMin, rect.yMin, z), ViewportToWorldPoint(rect.xMin, rect.yMax, z));
        Gizmos.DrawLine(ViewportToWorldPoint(rect.xMin, rect.yMax, z), ViewportToWorldPoint(rect.xMax, rect.yMax, z));
        Gizmos.DrawLine(ViewportToWorldPoint(rect.xMax, rect.yMax, z), ViewportToWorldPoint(rect.xMax, rect.yMin, z));
        Gizmos.DrawLine(ViewportToWorldPoint(rect.xMax, rect.yMin, z), ViewportToWorldPoint(rect.xMin, rect.yMin, z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ViewportToWorldPoint(0, playerHeight / (float) screenHeight, z),
            ViewportToWorldPoint(1, playerHeight / (float) screenHeight, z));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ViewportToWorldPoint(rect.xMin, (rect.yMin + rect.yMax) / 2, z), MaxTargetRadius);
    }

    private Vector3 ViewportToWorldPoint(float x, float y, float z = 0) {
        return camera.ViewportToWorldPoint(new Vector3(x, y, z));
    }

    private Vector3 ViewportToWorldPoint(Vector3 pos) {
        return ViewportToWorldPoint(pos.x, pos.y, pos.z);
    }
}
}

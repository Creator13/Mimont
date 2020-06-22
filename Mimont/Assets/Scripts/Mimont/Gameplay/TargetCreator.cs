using System;
using System.Collections;
using Mimont.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mimont.Gameplay {
internal class Visuals {
    public Vector2 basePos;
    public Vector2[] originalPositions = new Vector2[2];
    public Vector2[] remappedpositions = new Vector2[2];
    public float radius;
}

public class TargetCreator : MonoBehaviour {
    // TODO Test code
    private Visuals vis;

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

    private Rect SpawnRect { get; set; }
    private float SpawnRectDiagonal { get; set; }

    public bool Paused {
        get => paused;
        set {
            if (paused && !value) timeSinceLastSpawn = spawnRate;
            paused = value;
        }
    }

    public event Action<Vector3, Vector3, int, int> TargetCreated;

    private void Awake() {
        UpdateSpawnRect();
    }

    public void StartSpawning(int delay) {
        StartCoroutine(Countdown(delay, () => Paused = false));
    }

    private static IEnumerator Countdown(int seconds, Action callback) {
        while (seconds >= 0) {
            seconds--;
            yield return new WaitForSeconds(1);
        }

        callback();
    }

    private void Update() {
        if (Paused) return;

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > spawnRate) {
            timeSinceLastSpawn = 0;

            CreateTarget();
        }
    }

    private void CreateTarget() {
        var tier1 = Random.Range(0, tierSettings.tiers.Count);
        var tier2 = Random.Range(0, tierSettings.tiers.Count);

        // Random position within rectangle
        var basePos = new Vector2(
            Random.Range(SpawnRect.xMin, SpawnRect.xMax),
            Random.Range(SpawnRect.yMin, SpawnRect.yMax)
        );

        // Maximum distance between the points
        var radius = Mathf.Lerp(SpawnRectDiagonal, 0, GameTime.ElapsedNormalized);
        var positions = new Vector2[2];
        
#if UNITY_EDITOR
        vis = new Visuals {
            basePos = basePos,
            radius = radius
        };
#endif

        for (var i = 0; i < 2; i++) {
            // Generate random point within circle starting at basePos
            var circlePoint = Random.insideUnitCircle * radius;
            // Calculate the position of the point
            var posInCircle = basePos + circlePoint;

#if UNITY_EDITOR
            vis.originalPositions[i] = posInCircle;
#endif

            // Remap the x coordinate to within the original rectangle if if falls outside of it
            if (posInCircle.x < SpawnRect.xMin || posInCircle.x > SpawnRect.xMax) {
                posInCircle.x = Remap(posInCircle.x,
                    basePos.x - radius, basePos.x + radius,
                    SpawnRect.xMin, SpawnRect.xMax
                );
            }

            // Remap the y coordinate if it falls out of the rect
            if (posInCircle.y < SpawnRect.yMin || posInCircle.y > SpawnRect.yMax) {
                posInCircle.y = Remap(posInCircle.y,
                    basePos.y - radius, basePos.y + radius,
                    SpawnRect.yMin, SpawnRect.yMax
                );
            }

            positions[i] = posInCircle;
#if UNITY_EDITOR
            vis.remappedpositions[i] = posInCircle;
#endif
        }

        var pos1 = ViewportToWorldPoint(positions[0]);
        var pos2 = ViewportToWorldPoint(positions[1]);

        TargetCreated?.Invoke(pos1, pos2, tier1, tier2);
    }

    private void UpdateSpawnRect() {
        /* Viewport rect: (w,h)
         * (0,1)-----(1,1)
         *  |   [   ]   |
         *  |           |
         * (0,0)-----(1,0)
         */

        var playerHeightRelative = (float) playerHeight / screenHeight;
        var edgeClearance = EdgeClearance;

        SpawnRect = new Rect(edgeClearance, playerHeightRelative * .6f, 1 - edgeClearance * 2,
            playerHeightRelative * .45f);
        SpawnRectDiagonal = Mathf.Sqrt(SpawnRect.height * SpawnRect.height + SpawnRect.width * SpawnRect.width);
    }

    private void OnDrawGizmos() {
        var z = transform.position.z;

        UpdateSpawnRect();

        // draw viewport rect
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(ViewportToWorldPoint(0, 0, z), ViewportToWorldPoint(0, 1, z));
        Gizmos.DrawLine(ViewportToWorldPoint(0, 1, z), ViewportToWorldPoint(1, 1, z));
        Gizmos.DrawLine(ViewportToWorldPoint(1, 1, z), ViewportToWorldPoint(1, 0, z));
        Gizmos.DrawLine(ViewportToWorldPoint(1, 0, z), ViewportToWorldPoint(0, 0, z));

        // draw spawn rect
        var rect = SpawnRect;
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

        if (vis != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(ViewportToWorldPoint(vis.basePos), .1f);

            var distant = vis.basePos;
            distant.x += vis.radius;
            var distantWorld = ViewportToWorldPoint(distant);
            var baseWorld = ViewportToWorldPoint(vis.basePos);
            var radius = Vector3.Distance(baseWorld, distantWorld);

            Gizmos.DrawWireSphere(baseWorld, radius);

            Gizmos.color = Color.green;
            foreach (var pos in vis.originalPositions) {
                Gizmos.DrawSphere(ViewportToWorldPoint(pos), .1f);
            }

            Gizmos.color = Color.red;
            foreach (var pos in vis.remappedpositions) {
                Gizmos.DrawSphere(ViewportToWorldPoint(pos), .08f);
            }
        }
    }

    private Vector3 ViewportToWorldPoint(float x, float y, float z = 0) {
        return camera.ViewportToWorldPoint(new Vector3(x, y, z));
    }

    private Vector3 ViewportToWorldPoint(Vector3 pos) {
        return ViewportToWorldPoint(pos.x, pos.y, pos.z);
    }

    private static float Remap(float value, float low1, float high1, float low2, float high2) {
        return low2 + (high2 - low2) * ((value - low1) / (high1 - low1));
    }
}
}

using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mimont {
public class TargetSpawner : MonoBehaviour {
    [SerializeField] private new Camera camera;

    [Space(10)] [SerializeField] private TargetTierSettings tierSettings;
    [SerializeField] public float spawnRateMultiplier = 1;
    [SerializeField] public float maxTargetRadius = 10;

    [Space(10)] [Tooltip("In cm")] [SerializeField] private int playerHeight = 180;
    [Tooltip("In cm")] [SerializeField] private int screenHeight = 239;

    [Space(10)] [SerializeField] private Target targetPrefab;

    private Coroutine spawnCoroutine;

    private float CameraWidth => camera.orthographicSize * 2 * camera.aspect;
    private float EdgeClearance => maxTargetRadius / CameraWidth;

    public event System.Action<Target> TargetCreated;

    private void Start() {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
        Debug.Log(GetSpawnRect());
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            SpawnTarget();
            yield return new WaitForSeconds(5);
        }
    }

    private void SpawnTarget() {
        var target = Instantiate(targetPrefab, transform, false);
        target.Tier = tierSettings.tiers[Random.Range(0, tierSettings.tiers.Count)];
        target.maxRadius = maxTargetRadius;

        var spawnRect = GetSpawnRect();
        var viewportPos = new Vector2(
            Random.Range(spawnRect.xMin, spawnRect.xMax),
            Random.Range(spawnRect.yMin, spawnRect.yMax)
        );
        var pos = ViewportToWorldPoint(viewportPos);
        target.transform.localPosition = pos;

        TargetCreated?.Invoke(target);
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

#if UNITY_EDITOR
    private void OnValidate() {
        maxTargetRadius = Mathf.Clamp(maxTargetRadius, 0, CameraWidth / 2);
    }
#endif

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
        Gizmos.DrawLine(ViewportToWorldPoint(0, playerHeight / (float) screenHeight, z), ViewportToWorldPoint(1, playerHeight / (float) screenHeight, z));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ViewportToWorldPoint(rect.xMin, (rect.yMin + rect.yMax) / 2, z), maxTargetRadius);
    }

    private Vector3 ViewportToWorldPoint(float x, float y, float z = 0) {
        return camera.ViewportToWorldPoint(new Vector3(x, y, z));
    }

    private Vector3 ViewportToWorldPoint(Vector3 pos) {
        return ViewportToWorldPoint(pos.x, pos.y, pos.z);
    }
}
}

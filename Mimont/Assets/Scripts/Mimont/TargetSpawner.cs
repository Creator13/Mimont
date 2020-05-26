using System.Collections;
using UnityEngine;

namespace Mimont {
public class TargetSpawner : MonoBehaviour {
    [SerializeField] private TargetTierSettings tierSettings;
    [SerializeField] public float spawnRateMultiplier = 1;
    [Tooltip("In meters")] [SerializeField] private float playerHeight = 1.80f;

    [Space(10)] [SerializeField] private Target targetPrefab;
    
    private Coroutine spawnCoroutine;
    
    public event System.Action<Target> TargetCreated;

    private void Start() {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
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

        // Todo upgrade to better random position selection
        var pos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
        target.transform.localPosition = pos;
        
        TargetCreated?.Invoke(target);
    }
}
}

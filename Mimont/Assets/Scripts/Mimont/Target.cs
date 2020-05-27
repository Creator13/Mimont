using System;
using System.Collections;
using UnityEngine;

namespace Mimont {
[Serializable]
public class TargetTier {
    public Color color;
    [Tooltip("Point multiplication factor")] public float multiplier = 1f;
    [Tooltip("Spawns/second")] public float spawnRate = 1f;
}

public class Target : MonoBehaviour, ISphere {
    private static readonly Vector3 StartScale = new Vector3(.1f, .1f, .1f);
    private static readonly int UnlitColor = Shader.PropertyToID("_BaseColor");

    [SerializeField] private float growSpeed = 1;
    public float maxRadius;

    private TargetTier tier;

    private new Renderer renderer;
    private Renderer Renderer => renderer ? renderer : (renderer = GetComponent<Renderer>());

    public event Action<int> Caught;

    public TargetTier Tier {
        get => tier;
        set {
            tier = value;
            Color = tier.color;
        }
    }

    private Color Color {
        set {
            var matPropBlock = new MaterialPropertyBlock();
            matPropBlock.SetColor(UnlitColor, value);
            Renderer.SetPropertyBlock(matPropBlock);
        }
    }

    public float Radius => transform.localScale.x * .5f;
    public Vector3 Position => transform.position;

    private void Start() {
        transform.localScale = StartScale;
    }

    private void Update() {
        transform.localScale += new Vector3(growSpeed, growSpeed, growSpeed) * Time.deltaTime;
        transform.localScale = new Vector3(
            Mathf.Clamp(transform.localScale.x, 0, maxRadius * 2),
            Mathf.Clamp(transform.localScale.y, 0, maxRadius * 2),
            Mathf.Clamp(transform.localScale.z, 0, maxRadius * 2)
        );
    }

    public void Catch() {
        Caught?.Invoke(Mathf.RoundToInt(Radius * 10 * Tier.multiplier));

        StartCoroutine(Expand());
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy() {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    // ReSharper disable once IteratorNeverReturns
    private IEnumerator Expand() {
        while (true) {
            growSpeed *= 3;
            yield return null;
        }
    }
}
}

using System;
using UnityEngine;

namespace Mimont.Gameplay {
[Serializable]
public class TargetTier {
    [ColorUsage(true, true)] public Color color;
    [Tooltip("Point multiplication factor")] public float multiplier = 1f;
    public int stepCount = 10;
    [Tooltip("Spawns/second")] public float spawnRate = 1f;
}

[RequireComponent(typeof(TargetVisualController))]
public class Target : MonoBehaviour, ISphere {
    private static readonly Vector3 StartScale = new Vector3(.5f, .5f, .5f);
    private static readonly int UnlitColor = Shader.PropertyToID("_BaseColor");

    public event Action<int> Caught;

    [SerializeField] private float growSpeed = .2f;
    public float maxRadius;

    private float stepSize;
    private float touchingModifier = 1;

    public float TouchingModifier {
        get => touchingModifier;
        set {
            touchingModifier = value;
            Visuals.addedScale = stepSize * touchingModifier;
        }
    }

    private TargetTier tier;

    private TargetVisualController visuals;
    private TargetVisualController Visuals => visuals ? visuals : visuals = GetComponent<TargetVisualController>();

    public TargetTier Tier {
        get => tier;
        set {
            tier = value;
            Color = tier.color;
        }
    }

    private Color Color {
        set => Visuals.targetColor = value;
    }

    public float Radius => transform.localScale.x * .5f;

    public Vector3 Position => transform.position;
    // public float GrowSpeed => growSpeed * tier.multiplier;

    private void Start() {
        stepSize = (maxRadius * 2 - StartScale.x) / tier.stepCount;

        Visuals.startScale = StartScale;
        Visuals.maxScale = maxRadius * 2;
        Visuals.addedScale = stepSize;
        Visuals.StartSpawn();
    }

    public void Catch() {
        Visuals.StartHit(() => Caught?.Invoke(Mathf.RoundToInt(Radius * 10 * Tier.multiplier)));
    }
}
}

using System;
using UnityEngine;

namespace Mimont.Gameplay {
[Serializable]
public class TargetTier {
    [ColorUsage(true, true)] public Color color;
    public int minScore;
    public int maxScore;
    public int stepCount = 10;
    public float startRadius = .25f;
}

[RequireComponent(typeof(TargetVisualController))]
public class Target : MonoBehaviour, ISphere {
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

    private void Start() {
        stepSize = (maxRadius * 2 - tier.startRadius) / tier.stepCount;

        Visuals.startScale = new Vector3(tier.startRadius * 2, tier.startRadius * 2, tier.startRadius * 2);
        Visuals.maxScale = maxRadius * 2;
        Visuals.addedScale = stepSize;
        Visuals.StartSpawn();
    }

    public void Catch() {
        Visuals.StartHit(() => {
            Caught?.Invoke(Mathf.RoundToInt(
                Mathf.Lerp(tier.maxScore, tier.minScore, (Radius - tier.startRadius / 2) / maxRadius)
            ));
        });
    }
}
}

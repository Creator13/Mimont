using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable Unity.PreferNonAllocApi

namespace Mimont.Gameplay {
[RequireComponent(typeof(RingVisuals))]
public class Ring : MonoBehaviour, ISphere {
    [SerializeField] private float growSpeed = 1;
    [SerializeField] private float playerTouchModifier = .3f;
    [SerializeField] private float opponentTouchModifier = .65f;

    private RingVisuals visuals;

    private new bool enabled;
    private bool isPlayer;
    private List<Target> touching = new List<Target>(20);
    private readonly List<Target> encapsulated = new List<Target>(20);

    private bool Enabled {
        get => enabled;
        set {
            if (enabled && !value) {
                visuals.Destroy();
                Radius = 0;
            }
            enabled = value;
        }
    }

    public bool IsPlayer {
        get => isPlayer;
        set {
            isPlayer = value;
            if (!isPlayer) {
                visuals.startColors = new[] {Color.gray};
            }
        }
    }

    public float Radius { get; private set; }
    public Vector3 Position => transform.position;
    private float TouchModifier => IsPlayer ? playerTouchModifier : opponentTouchModifier;

    public SpherePool SpherePool {
        set => visuals.icospherePool = value;
    }

    private void Awake() {
        visuals = GetComponent<RingVisuals>();
        Enabled = false;
    }

    public void Activate(Vector3 pos) {
        transform.localPosition = pos;
        visuals.Spawn(pos);
        Enabled = true;
    }

    public void Release() {
        if (IsPlayer) {
            foreach (var target in encapsulated) {
                target.Catch();
            }
            encapsulated.Clear();
        }

        Enabled = false;
    }

    private void Update() {
        UpdateTouchingTargets();
        UpdateEncapsulated();

        if (!Enabled) return;

        Radius += growSpeed / 2 * Time.deltaTime;
        visuals.UpdateRadius(Radius);
    }

    private void UpdateTouchingTargets() {
        // Load all colliders
        var colliders = Enabled ? Physics.OverlapSphere(Position, Radius) : new Collider[0];

        // Get only Target gameObjects
        var newTouching = new List<Target>(colliders.Length);
        newTouching.AddRange(colliders.Select(collider => collider.GetComponent<Target>()).Where(target => target));

        // Unset all old touched targets if changed
        var old = touching.Except(newTouching).ToList();
        if (old.Count > 0) {
            old.ForEach(t => t.touchingModifier = 1);
        }

        if (newTouching.Count > 0) {
            // Set all newly touched targets if changed
            newTouching.Except(touching).ToList().ForEach(t => t.touchingModifier = TouchModifier);
        }

        // Replace touch
        touching = newTouching;
    }

    private void UpdateEncapsulated() {
        // Get all the targets that are not yet encapsulated
        var canidates = touching.Except(encapsulated).ToList();

        // Add the encapsulated targets to the list when they are inside the ring
        foreach (var target in canidates.Where(target => target.IsInside(this))) {
            encapsulated.Add(target);
            if (isPlayer) {
                visuals.UpdateColor(target.Tier.color);
            }
        }
    }
}
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mimont.Gameplay {
public class Ring : MonoBehaviour, ISphere {
    private static readonly Vector3 StartScale = new Vector3(.1f, .1f, .1f);

    [SerializeField] private float growSpeed = 1;

    private new Renderer renderer;
    private Renderer Renderer => renderer ? renderer : renderer = GetComponent<Renderer>();

    private new bool enabled;
    private bool isPlayer;
    private List<Target> touching = new List<Target>(20);

    private bool Enabled {
        get => enabled;
        set {
            enabled = value;
            Renderer.enabled = value;
        }
    }

    public float Radius => transform.localScale.x * .5f;
    public Vector3 Position => transform.position;

    private void Awake() {
        Enabled = false;
    }

    public void Activate(Vector3 pos, bool isPlayer = false) {
        this.isPlayer = isPlayer;
        transform.localPosition = pos;
        transform.localScale = StartScale;
        Enabled = true;
    }

    public void Release() {
        if (isPlayer) {
            var targets = Physics.OverlapSphere(transform.position, Radius);
            foreach (var collider in targets) {
                var target = collider.GetComponent<Target>();
                if (target && target.IsInside(this)) {
                    target.Catch();
                }
            }
        }

        Enabled = false;
    }

    private void Update() {
        UpdateTocuhingTargets();

        if (!Enabled) return;

        transform.localScale += new Vector3(growSpeed, growSpeed, growSpeed) * Time.deltaTime;
    }

    private void UpdateTocuhingTargets() {
        // Load all colliders
        var colliders = Enabled ? Physics.OverlapSphere(Position, Radius) : new Collider[0];

        // Get only Target gameobjects
        var newTouching = new List<Target>(colliders.Length);
        newTouching.AddRange(colliders.Select(collider => collider.GetComponent<Target>()).Where(target => target));

        // Unset all old touched targets if changed
        var old = touching.Except(newTouching).ToList();
        if (old.Count > 0) {
            old.ForEach(t => t.touching = false);
        }

        if (newTouching.Count > 0) {
            // Set all newly touched targets if changed
            newTouching.Except(touching).ToList().ForEach(t => t.touching = true);
        }

        // Replace touch
        touching = newTouching;
    }
}
}

using UnityEngine;

namespace Mimont {
public class Ring : MonoBehaviour, ISphere {
    private static readonly Vector3 StartScale = new Vector3(.1f, .1f, .1f);

    [SerializeField] private float growSpeed = 1;

    private new bool enabled;

    public bool Enabled {
        get => enabled;
        set {
            enabled = value;
            gameObject.SetActive(value);
        }
    }

    public float Radius => transform.localScale.x * .5f;
    public Vector3 Position => transform.position;

    private void Awake() {
        Enabled = false;
    }

    public void Activate(Vector3 pos) {
        transform.localPosition = pos;
        transform.localScale = StartScale;
        Enabled = true;
    }

    public void Release() {
        var targets = Physics.OverlapSphere(transform.position, Radius);
        foreach (var collider in targets) {
            var target = collider.GetComponent<Target>();
            if (target && target.IsInside(this)) {
                target.Catch();
            }
        }

        Enabled = false;
    }

    private void Update() {
        if (!Enabled) return;

        transform.localScale += new Vector3(growSpeed, growSpeed, growSpeed) * Time.deltaTime;
    }
}
}

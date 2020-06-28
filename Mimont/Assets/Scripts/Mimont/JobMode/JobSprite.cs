using UnityEngine;

namespace Mimont.JobMode {
public class JobSprite : MonoBehaviour {
    [SerializeField] private Sprite idle1;
    [SerializeField] private Sprite idle2;
    [SerializeField] private Sprite kiss1;
    [SerializeField] private Sprite kiss2;

        [SerializeField] private float idleShrink = 0.8f;

    private Vector3 initialScale;
    private new SpriteRenderer renderer;
    private SpriteRenderer Renderer => renderer ? renderer : renderer = GetComponent<SpriteRenderer>();

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        public void SetIdle() {
        Renderer.sprite = Random.value < .5f ? idle1 : idle2;
            transform.localScale = initialScale * idleShrink;

    }

    public void SetKiss() {
        Renderer.sprite = Random.value < .5f ? kiss1 : kiss2;
            transform.localScale = initialScale;
        }

    private void OnValidate() {
        if (idle1) {
            Renderer.sprite = idle1;
        }
        else if (idle2) {
            Renderer.sprite = idle2;
        }
        else {
            Renderer.sprite = null;
        }
    }
}
}

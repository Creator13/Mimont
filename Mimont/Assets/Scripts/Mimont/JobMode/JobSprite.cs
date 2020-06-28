using UnityEngine;

namespace Mimont.JobMode {
public class JobSprite : MonoBehaviour {
    [SerializeField] private Sprite idle1;
    [SerializeField] private Sprite idle2;
    [SerializeField] private Sprite kiss1;
    [SerializeField] private Sprite kiss2;

    private SpriteRenderer renderer;
    private SpriteRenderer Renderer => renderer ? renderer : GetComponent<SpriteRenderer>();

    public void SetIdle() {
        Renderer.sprite = Random.value < .5f ? idle1 : idle2;
    }

    public void SetKiss() {
        Renderer.sprite = Random.value < .5f ? kiss1 : kiss2;
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

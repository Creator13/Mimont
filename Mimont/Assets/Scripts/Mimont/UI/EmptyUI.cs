using UnityEngine;

namespace Mimont.UI {
public class EmptyUI : MonoBehaviour, UIScreen {
    public bool Active {
        set => gameObject.SetActive(value);
    }
}
}

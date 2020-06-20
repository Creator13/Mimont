using UnityEngine;

namespace Mimont.UI {
public class EmptyUI : MonoBehaviour, IUIScreen {
    public bool Active {
        set => gameObject.SetActive(value);
    }
}
}

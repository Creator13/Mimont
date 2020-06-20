using TMPro;
using UnityEngine;

namespace Mimont.UI {
public class StartUI : MonoBehaviour, IUIScreen {
    [SerializeField] private MimontGame gameRoot;
    [SerializeField] private TMP_InputField ipInput;
    
    public bool Active {
        set => gameObject.SetActive(value);
    }

    public void HostGame() {
        gameRoot.Connect(true);
    }
    
    public void JoinGame() {
        gameRoot.Connect(false, ipInput.text);
    }
}
}

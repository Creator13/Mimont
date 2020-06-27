using System;
using UnityEngine;

namespace Mimont.UI {
public class MimontUI : MonoBehaviour {
    public event Action MenuUIRequested;
    
    [SerializeField] private EmptyUI gameUI;
    [SerializeField] private StartUI startUI;
    [SerializeField] private MessageUI messageUI;

    private IUIScreen activeUI;

    private IUIScreen ActiveUI {
        get => activeUI;
        set {
            if (activeUI != null) activeUI.Active = false;
            activeUI = value;
            if (activeUI != null) activeUI.Active = true;
        }
    }

    private void Awake() {
        gameUI.Active = false;
        startUI.Active = false;
        messageUI.Active = false;
    }

    private void Start() {
        ActiveUI = startUI;
    }

    public void OpenMenuUI() {
        MenuUIRequested?.Invoke();
        ActiveUI = startUI;
    }

    public void OpenGameUI() {
        ActiveUI = gameUI;
    }

    public void ShowMessage(int i) {
        if (!messageUI) return;

        ActiveUI = messageUI;
        messageUI.SetButtonOptions();
        messageUI.SetMessage(i);
    }

    public void ShowMessage(string messageText, params MessageUI.ButtonOptions[] options) {
        if (!messageUI) return;

        ActiveUI = messageUI;
        messageUI.SetButtonOptions(options);
        messageUI.SetMessage(messageText);
    }
}
}

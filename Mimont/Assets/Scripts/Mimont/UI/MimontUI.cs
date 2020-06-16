using System;
using UnityEngine;

namespace Mimont.UI {
public class MimontUI : MonoBehaviour {
    [SerializeField] private EmptyUI gameUI;
    [SerializeField] private EmptyUI startUI;
    [SerializeField] private MessageUI messageUI;

    private UIScreen activeUI;

    private UIScreen ActiveUI {
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

    public void OpenGameUI() {
        ActiveUI = gameUI;
    }

    public void ShowMessage(string messageText, params MessageUI.ButtonOptions[] options) {
        if (!messageUI) return;

        ActiveUI = messageUI;
        messageUI.SetMessage(messageText);
        messageUI.SetButtonOptions(options);
    }
}
}

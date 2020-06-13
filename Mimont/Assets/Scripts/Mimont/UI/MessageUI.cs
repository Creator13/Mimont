using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimont.UI {
public class MessageUI : MonoBehaviour, UIScreen {
    public enum ButtonOptions {Quit, MainMenu}
    
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button menuButton;

    public List<Button> Buttons => new List<Button> {quitButton, menuButton};
    
    private bool active;
    public bool Active {
        get => active;
        set {
            active = value;
            gameObject.SetActive(value);
            // Clear text when activated
            if (value) text.text = "";
        }
    }

    public void SetMessage(string message) {
        text.text = message;
    }

    public void SetButtonOptions(params ButtonOptions[] options) {
        Buttons.ForEach(button => button.gameObject.SetActive(false));
        foreach (var option in options.Distinct()) {
            switch (option) {
                case ButtonOptions.Quit:
                    quitButton.gameObject.SetActive(true);
                    break;
                case ButtonOptions.MainMenu:
                    menuButton.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
}

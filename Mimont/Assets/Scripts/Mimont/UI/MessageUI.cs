using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mimont.UI {
public class MessageUI : MonoBehaviour, IUIScreen {
    public enum ButtonOptions { Quit, MainMenu }

    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject[] KTP;
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
            text.enabled = true;
            text.text = message;
    }

    public void SetMessage(int i) {
        if (i < KTP.Length && i >= 0) {
            text.enabled = false;
            KTP[i].SetActive(true);
        }
        else if (i == KTP.Length) {
            text.enabled = false;
            foreach (var t in KTP) {
                t.SetActive(false);
            }
        }
        else {
            Debug.LogError($"i = {i}, i is out of range");
        }
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

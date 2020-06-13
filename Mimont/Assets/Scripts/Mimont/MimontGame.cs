﻿using System;
using System.Collections;
using Mimont.Netcode;
using Mimont.UI;
using UnityEngine;

namespace Mimont {
public class MimontGame : MonoBehaviour {
    private MimontServer server;
    private MimontClient client;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Player player;
    [SerializeField] private MimontUI ui;

    private bool isServer;

    private bool paused;

    public bool Paused {
        get => paused;
        set {
            paused = value;
            inputHandler.gameObject.SetActive(value);
            player.Paused = value;
        }
    }

    private void Awake() {
        inputHandler.gameObject.SetActive(false);
        player.Active = false;
    }

    private void OnDestroy() {
        client?.Dispose();
        server?.Stop();
    }

    private void Update() {
        if (isServer && server.IsRunning) {
            server.Update();
        }

        if (client != null && client.Started) {
            client.Update();
        }
    }

    public void Connect(bool isHost) {
        if (client != null) {
            return;
        }

        isServer = isHost;
        // If starting as server, awaken server
        if (isServer) {
            server?.Stop();
            server = new MimontServer();
            server.Start();
        }

        // Awaken client
        client?.Dispose();
        client = new MimontClient();
        client.Connect();

        client.StartGame += () => StartCoroutine(Countdown(3, StartGame));

        // Link all client callbacks
        client.PlayerLeft += () => {
            Paused = true;
            ui.ShowMessage("Other player left game...", MessageUI.ButtonOptions.MainMenu);
        };
        client.Disconnected += () => {
            Paused = true;
            ui.ShowMessage("Disconnected...", MessageUI.ButtonOptions.Quit, MessageUI.ButtonOptions.MainMenu);
        };
    }

    private IEnumerator Countdown(int seconds, Action callback) {
        while (seconds > 0) {
            ui.ShowMessage($"{seconds}");
            seconds--;
            yield return new WaitForSeconds(1);
        }

        callback();
    }
    
    private void StartGame() {
        // Switch UI
        ui.OpenGameUI();

        // Wake up objects
        inputHandler.gameObject.SetActive(true);
        player.Active = true;
    }
}
}

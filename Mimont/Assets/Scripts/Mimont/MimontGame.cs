using System;
using System.Collections;
using Mimont.Gameplay;
using Mimont.Netcode;
using Mimont.UI;
using Networking.Server;
using UnityEngine;
using Player = Mimont.Gameplay.Player;

namespace Mimont {
public class MimontGame : MonoBehaviour {
    private Server server;
    private MimontClient client;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Player player;
    [SerializeField] private TargetCreator targetCreator;
    [SerializeField] private MimontUI ui;

    [Space] [SerializeField] private bool debugMode;

    private bool isServer;

    private bool paused;

    private bool Paused {
        get => paused;
        set {
            paused = value;
            if (inputHandler) inputHandler.gameObject.SetActive(value);
        }
    }

    private void Awake() {
        inputHandler.gameObject.SetActive(false);

        if (Application.isMobilePlatform) {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
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

    public void Connect(bool isHost, string ipAddress = "") {
        if (client != null) {
            return;
        }

        isServer = isHost;
        // If starting as server, awaken server
        if (isServer) {
            server?.Stop();
            
            if (debugMode) {
                server = new MimontServerDebug(targetCreator);
            }
            else {
                server = new MimontServer(targetCreator);
            }

            server.Start();
        }

        // Awaken client
        StartClient(player, ipAddress);
    }

    private void StartClient(Player player, string ipAddress) {
        client?.Dispose();
        client = new MimontClient {Player = player};
        client.Connect(ipAddress);

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

        ui.ShowMessage("Waiting for other player...");
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
    }
}
}

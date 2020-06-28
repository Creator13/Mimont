using System;
using System.Collections;
using Hellmade.Sound;
using Mimont.Gameplay;
using Mimont.JobMode;
using Mimont.Netcode;
using Mimont.UI;
using Networking.Server;
using UnityEngine;
using Player = Mimont.Gameplay.Player;

namespace Mimont {
[RequireComponent(typeof(GameTime))]
public class MimontGame : MonoBehaviour {
    private Server server;
    private MimontClient client;
    private MimontClient jobClient;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Player player;
    [SerializeField] private TargetCreator targetCreator;
    [SerializeField] private MimontUI ui;
    [SerializeField] private TimeDisplay timeDisplay;
    [SerializeField] private AudioClip music;
    [SerializeField] private bool playMusic;

    [Space] [SerializeField] private bool debugMode;
    [SerializeField] private bool jobMode;
    [SerializeField] private JobSprite jobSprite;

    private GameTime timer;
    private GameTime Timer => timer ? timer : timer = GetComponent<GameTime>();

    private bool isServer;
    private bool paused;
    private int audioID;

    //BPM timer
    public static event Action OnBeat;

    public float bpm = 30;
    private float timeBetweenBeat = 0;

    private bool Paused {
        get => paused;
        set {
            paused = value;
            if (inputHandler) inputHandler.gameObject.SetActive(!value);
            Timer.Running = !value;
            if (jobMode && jobSprite) jobSprite.gameObject.SetActive(!value);
        }
    }

    private void Awake() {
        Paused = true;

        if (Application.isMobilePlatform) {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        ui.MenuUIRequested += ResetForMenu;

        if (jobMode) debugMode = false;
        if (!jobMode) jobSprite.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        client?.Dispose();
        jobClient?.Dispose();
        server?.Stop();
    }

    private void Update() {
        if (isServer && server != null && server.IsRunning) {
            server.Update();
        }

        if (client != null && client.Started) {
            client.Update();
        }

        if (jobClient != null && jobClient.Started) {
            jobClient.Update();
        }

        BPMTimer();
    }

    private void ResetForMenu() {
        Paused = true;
        client?.Dispose();
        jobClient?.Dispose();
        server?.Stop();

        client = null;
        jobClient = null;
        server = null;

        var musicAudio = EazySoundManager.GetMusicAudio(music);
        musicAudio?.Stop();
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
                server = new MimontServerDebug(targetCreator, GameTime.GameLength);
            }
            else {
                server = new MimontServer(targetCreator, GameTime.GameLength);
            }

            server.Start();
        }

        // Awaken client
        StartClient(player, ipAddress);

        if (!debugMode && !jobMode) {
            ui.ShowMessage("Waiting for other player...", MessageUI.ButtonOptions.MainMenu);
        }

        if (jobMode) {
            UnleashJob();
        }
    }

    private void StartClient(Player player, string ipAddress) {
        client?.Dispose();
        client = new MimontClient {Player = player};
        client.Connect(ipAddress);

        client.StartGame += () => {
            if (playMusic) {
                audioID = EazySoundManager.PlayMusic(music, .15f, true, false, 3, 3);
            }

            StartCoroutine(Countdown(3, StartGame));
        };

        // Link all client callbacks
        client.PlayerLeft += () => {
            Paused = true;
            ui.ShowMessage("Other player left game...", MessageUI.ButtonOptions.MainMenu);
        };
        client.Disconnected += () => {
            Paused = true;
            ui.ShowMessage("Disconnected...", MessageUI.ButtonOptions.MainMenu);
        };
        client.GameWon += () => {
            player.gameObject.SetActive(false);
            Paused = true;
            ui.ShowMessage("How did that feel?", MessageUI.ButtonOptions.MainMenu);
        };
        client.GameLost += () => {
            var score = player.Score;
            player.gameObject.SetActive(false);
            Paused = true;
            ui.ShowMessage($"Time up!\n {score}", MessageUI.ButtonOptions.MainMenu);
        };
    }

    private void UnleashJob() {
        var job = new GameObject("Job AI");
        var jobAI = job.AddComponent<JobAI>();
        jobAI.sprite = jobSprite;

        jobClient?.Dispose();
        jobClient = new MimontClient {Player = jobAI};
        jobClient.Connect();
    }

    private IEnumerator Countdown(int seconds, Action callback) {
        int i = 0;
        while (seconds > 0) {
            ui.ShowMessage(i);
            i++;
            seconds--;
            yield return new WaitForSeconds(1);
        }

        callback();
    }

    private void StartGame() {
        Paused = false;
        Timer.Reinitialize();
        timeBetweenBeat = .000001f;

        // Switch UI
        ui.OpenGameUI();

        // Wake up objects
        inputHandler.gameObject.SetActive(true);
    }

    private void BPMTimer() {
        var startTimeBtwBeat = 60f / bpm;

        if (timeBetweenBeat <= 0) {
            OnBeat?.Invoke();
            timeBetweenBeat = startTimeBtwBeat + timeBetweenBeat;
        }

        timeBetweenBeat -= Time.unscaledDeltaTime;
    }
}
}

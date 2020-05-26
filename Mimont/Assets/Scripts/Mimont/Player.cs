using System;
using UnityEngine;

namespace Mimont {
public class Player : MonoBehaviour {
    [SerializeField] private RingManager ringManager;
    [SerializeField] private TargetSpawner targetSpawner;

    public event Action<float> ScoreChanged;
    
    private int score;

    public int Score {
        get => score;
        private set {
            score = value;
            ScoreChanged?.Invoke(score);
        }
    }

    private void Awake() {
        targetSpawner.TargetCreated += SubscribeToTarget;
    }

    private void SubscribeToTarget(Target t) {
        t.Caught += AddScore;
    }

    private void AddScore(int points) {
        Score += points;
    }
}
}

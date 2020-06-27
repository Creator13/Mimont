using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mimont.Gameplay {
public class Player : MonoBehaviour {
    [SerializeField] private Target targetPrefab;
    [SerializeField] private TargetTierSettings targetTierSettings;
    [SerializeField] private RingManager ringManager;

    public event Action<Vector3> RingCreated;
    public event Action RingReleased;

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
        ringManager.RingCreated += PropagateRingCreated;
        ringManager.RingReleased += PropagateRingReleased;
    }

    private void OnDestroy() {
        ringManager.RingCreated -= PropagateRingCreated;
        ringManager.RingReleased -= PropagateRingReleased;
    }

    public void AddTarget(Vector3 pos, int tierIndex) {
        var target = Instantiate(targetPrefab, transform, false);
        target.Tier = targetTierSettings.tiers[tierIndex];

        target.transform.localPosition = pos;
        SubscribeToTarget(target);
    }

    public void StartOtherRing(Vector3 position) {
        ringManager.StartOpponentRing(position);
    }

    public void ReleaseOtherRing() {
        ringManager.ReleaseOpponentRing();
    }

    private void SubscribeToTarget(Target t) {
        t.Caught += AddScore;
    }

    private void AddScore(int points) {
        Score += points;
    }

    private void PropagateRingCreated(Vector3 pos) {
        RingCreated?.Invoke(pos);
    }

    private void PropagateRingReleased() {
        RingReleased?.Invoke();
    }
}
}

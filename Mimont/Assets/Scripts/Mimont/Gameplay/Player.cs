using System;
using UnityEngine;

namespace Mimont.Gameplay {
public class Player : MonoBehaviour {
    [SerializeField] private Target targetPrefab;
    [SerializeField] private TargetTierSettings targetTierSettings;
    [SerializeField] private RingManager ringManager;

    public event Action<float> ScoreChanged;
    
    private int score;

    public int Score {
        get => score;
        private set {
            score = value;
            ScoreChanged?.Invoke(score);
        }
    }

    public bool Active {
        set {
            ringManager.gameObject.SetActive(value);
            gameObject.SetActive(value);
        }
    }

    public void AddTarget(Vector3 position, int tierIndex) {
        var target = Instantiate(targetPrefab, transform, false);
        target.Tier = targetTierSettings.tiers[tierIndex];
        
        target.transform.localPosition = position;
        SubscribeToTarget(target);
    }

    private void SubscribeToTarget(Target t) {
        t.Caught += AddScore;
    }

    private void AddScore(int points) {
        Score += points;
    }
}
}

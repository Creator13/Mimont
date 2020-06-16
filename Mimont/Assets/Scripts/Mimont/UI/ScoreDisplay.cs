using Mimont.Gameplay;
using TMPro;
using UnityEngine;

namespace Mimont.UI {
[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour {
    [SerializeField] private Player player;
    private TMP_Text text;
    private TMP_Text Text => text ? text : (text = GetComponent<TMP_Text>());
    
    private void Awake() {
        player.ScoreChanged += SetScore;
    }

    private void Start() {
        SetScore(player.Score);
    }

    private void SetScore(float val) {
        Text.text = $"Score: {val}";
    }
}
}

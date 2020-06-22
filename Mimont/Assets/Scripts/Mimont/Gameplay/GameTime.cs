using System;
using UnityEngine;

namespace Mimont.Gameplay {
public class GameTime : MonoBehaviour {
    public static float Elapsed { get; private set; }
    public static float GameLength { get; private set; }
    public static float ElapsedNormalized => Elapsed / GameLength;
     
    [Tooltip("In minutes")] [SerializeField] private float gameLength;

    public bool Running { get; set; }

    private void Awake() {
        GameLength = gameLength * 60;
    }

    private void Update() {
        if (!Running) return;

        Elapsed += Time.deltaTime;
    }

    public void Reinitialize() {
        Elapsed = 0;
    }
}
}

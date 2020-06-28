using UnityEngine;
using TMPro;

namespace Mimont.Gameplay {
[RequireComponent(typeof(GameTime))]
public class TimeDisplay : MonoBehaviour {
    [SerializeField] private string minutesFormat;
    [SerializeField] private string secondsFormat;
    [SerializeField] private TMP_Text text;

    private void Update() {
        float displayTime = GameTime.GameLength - GameTime.Elapsed;

        string minutes = Mathf.Floor(displayTime / 60).ToString(minutesFormat);
        string seconds = Mathf.Floor(displayTime % 60).ToString(secondsFormat);

        text.text = $"{minutes}:{seconds}";
    }
}
}

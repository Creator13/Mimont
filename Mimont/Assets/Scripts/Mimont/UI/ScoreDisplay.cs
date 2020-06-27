using Mimont.Gameplay;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Mimont.UI {
[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour {
    private static readonly int FaceColor = Shader.PropertyToID("_FaceColor");
    private static readonly int Color = Shader.PropertyToID("_Color");

    [SerializeField] private Player player;
    [SerializeField] private RingManager ringManager;
    [SerializeField] private Material scoreMat;
    [SerializeField] private Material timeMat;
    [SerializeField] private Material mimontMat;
    [SerializeField] private Material newGameMat;
    [SerializeField] private float colorLerpDuration;
    [SerializeField] private AnimationCurve colorLerpCurve;
    [SerializeField] private AnimationCurve textLerpCurve;
    private TMP_Text text;
    private int displayedScore;

    private TMP_Text Text => text ? text : (text = GetComponent<TMP_Text>());

    private void Awake() {
        player.ScoreChanged += SetScore;
    }

    private void Start() {
        Text.text = $"{player.Score}";

        scoreMat.SetColor(FaceColor, mimontMat.GetColor(Color));
        timeMat.SetColor(FaceColor, newGameMat.GetColor(Color));
    }

    private void SetScore(float val) {
        //Text.text = $"{val}";
        UpdateScore();
    }

    private void UpdateScore() {
        var uniqueColors = ringManager.capturedColors.Distinct().ToList();
        ringManager.capturedColors.Clear();

        if (uniqueColors.Count == 0) {
            return;
        }
        else if (uniqueColors.Count == 1) {
            StartCoroutine(LerpColor(scoreMat, uniqueColors[0]));
        }
        else {
            int i = Random.Range(0, uniqueColors.Count);
            StartCoroutine(LerpColor(scoreMat, uniqueColors[i]));
            uniqueColors.RemoveAt(i);
            StartCoroutine(LerpColor(timeMat, uniqueColors[Random.Range(0, uniqueColors.Count)]));
        }
    }

    private IEnumerator LerpColor(Material mat, Color newCol) {
        float _timeValue = 0;

        var _oldCol = mat.GetColor(FaceColor);
        var newScore = player.Score;
        int updatedScore = displayedScore;

        while (_timeValue < 1) {
            _timeValue += Time.deltaTime / colorLerpDuration;
            var _evaluatedColorValue = colorLerpCurve.Evaluate(_timeValue);
            var _evaluatedTextValue = textLerpCurve.Evaluate(_timeValue);
            var _newCol = Vector4.Lerp(_oldCol, newCol, _evaluatedColorValue);
            updatedScore = Mathf.FloorToInt(Mathf.Lerp(displayedScore, newScore, _evaluatedTextValue));

            mat.SetColor(FaceColor, _newCol);
            Text.text = $"{updatedScore}";

            yield return null;
        }

        displayedScore = updatedScore;

        yield return null;
    }
}
}

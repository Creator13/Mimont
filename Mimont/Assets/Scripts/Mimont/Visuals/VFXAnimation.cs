using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class VFXAnimation : MonoBehaviour {
    [Header("Death Animation")] [SerializeField] private float deathMinSizeAnimDuration;
    [SerializeField] private AnimationCurve deathMinSizeAnimCurve;
    [SerializeField] private float deathMaxSizeAnimDuration;
    [SerializeField] private AnimationCurve deathMaxSizeAnimCurve;
    [SerializeField] private float deathFadeAnimDuration;
    [SerializeField] private AnimationCurve deathFadeAnimCurve;
    [SerializeField] private float minDeathSize;
    [SerializeField] private float maxDeathSize;
    [SerializeField] private Vector4 deathFadeColor;

    [Header("Spawn Animation")] [SerializeField] private float initialSize;
    [SerializeField] private float endSize;
    [SerializeField] private AnimationCurve spawnSizeCurve;
    [SerializeField] private float spawningAnimDuration;
    [SerializeField] private float startCount;
    [SerializeField] private float endCount;

    [Header("Growth")] [SerializeField] private float minSize;
    [SerializeField] private float maxSize;

    [SerializeField] private float growthRate;

    //[SerializeField, GradientUsage(true)] private UnityEngine.Gradient deathFadeGradient;
    //private UnityEngine.GradientColorKey[] deathFadeColorKeys;
    //private UnityEngine.GradientAlphaKey[] deathFadeAlphaKeys;
    private Coroutine deathAnimRoutine;
    private Coroutine spawnAnimRoutine;

    private VisualEffect visualEffect;


    // Start is called before the first frame update
    void Start() {
        visualEffect = GetComponent<VisualEffect>();

        //deathFadeColorKeys = deathFadeGradient.colorKeys;
        //deathFadeAlphaKeys = deathFadeGradient.alphaKeys;
    }

    private void Update() {
        if (Keyboard.current.dKey.wasPressedThisFrame) {
            PlayDeathAnim(deathMinSizeAnimDuration, deathMaxSizeAnimDuration, deathFadeAnimDuration, minDeathSize,
                maxDeathSize, deathFadeColor);
        }

        if (Keyboard.current.sKey.wasPressedThisFrame) {
            PlaySpawnAnim(spawningAnimDuration, initialSize, endSize);
        }

        if (Keyboard.current.gKey.isPressed) {
            Growth(false, maxSize, growthRate);
        }
        else {
            Growth(true, maxSize, growthRate);
        }
    }

    public void Growth(bool isPauzed, float maxSize, float growthRate) {
        var _newSize = visualEffect.GetFloat("Size");

        if (!isPauzed && _newSize <= maxSize) {
            _newSize += Time.deltaTime * growthRate;
        }

        visualEffect.SetFloat("Size", _newSize);
    }

    public void PlaySpawnAnim(float spawnDuration, float initialSize, float endSize) {
        if (deathAnimRoutine != null) {
            StopCoroutine(spawnAnimRoutine);
        }

        spawnAnimRoutine = StartCoroutine(SpawnAnimIE(spawnDuration, initialSize, endSize));
    }

    private IEnumerator SpawnAnimIE(float spawnDuration, float initialSize, float endSize) {
        visualEffect.enabled = true;

        visualEffect.SetFloat("ParticleCount", startCount);

        float _timeValue = 0;

        while (_timeValue < 1) {
            _timeValue += Time.deltaTime / spawnDuration;
            var _evaluatedTimeValue = spawnSizeCurve.Evaluate(_timeValue);
            var _newSize = Mathf.Lerp(initialSize, endSize, _evaluatedTimeValue);

            visualEffect.SetFloat("Size", _newSize);

            yield return null;
        }

        visualEffect.SetFloat("ParticleCount", endCount);

        yield return null;
    }

    public void PlayDeathAnim(float deathMinSizeAnimDuration, float deathMaxSizeAnimDuration,
        float deathFadeAnimDuration, float minDeathSize, float maxDeathSize, Vector4 deathFadeColor) {
        if (deathAnimRoutine != null) {
            StopCoroutine(deathAnimRoutine);
        }

        deathAnimRoutine = StartCoroutine(DeathAnimIE(deathMinSizeAnimDuration, deathMaxSizeAnimDuration,
            deathFadeAnimDuration, minDeathSize, maxDeathSize, deathFadeColor));
    }


    private IEnumerator DeathAnimIE(float minDuration, float maxDuration, float fadeDuration, float minSize,
        float maxSize, Vector4 fadeColor) {
        var _currentSize = visualEffect.GetFloat("Size");
        float _minTimeValue = 0;

        while (_minTimeValue < 1) {
            _minTimeValue += Time.deltaTime / minDuration;
            var _evaluatedTimeValue = deathMinSizeAnimCurve.Evaluate(_minTimeValue);
            var _newSize = Mathf.Lerp(_currentSize, minSize, _evaluatedTimeValue);

            visualEffect.SetFloat("Size", _newSize);

            yield return null;
        }

        _currentSize = visualEffect.GetFloat("Size");
        float _maxTimeValue = 0;

        while (_maxTimeValue < 1) {
            _maxTimeValue += Time.deltaTime / maxDuration;
            var _evaluatedTimeValue = deathMaxSizeAnimCurve.Evaluate(_minTimeValue);
            var _newSize = Mathf.Lerp(_currentSize, maxSize, _evaluatedTimeValue);

            visualEffect.SetFloat("Size", _newSize);

            yield return null;
        }

        //UnityEngine.Gradient _currentGradient = vE.GetGradient("Gradient");
        //UnityEngine.GradientColorKey[] _currentColorKeys = _currentGradient.colorKeys;
        //UnityEngine.GradientAlphaKey[] _currentAlphaKeys = _currentGradient.alphaKeys;
        //float _colorTimeValue = 0;

        //while (_colorTimeValue < 1)
        //{
        //    _colorTimeValue += Time.deltaTime / deathFadeAnimDuration;
        //    float _evaluatedTimeValue = deathFadeAnimCurve.Evaluate(_minTimeValue);
        //    UnityEngine.Gradient _newGradient = new UnityEngine.Gradient();
        //    UnityEngine.GradientColorKey[] _newColorKeys = new UnityEngine.GradientColorKey[_currentColorKeys.Length];
        //    UnityEngine.GradientAlphaKey[] _newAlphaKeys = new UnityEngine.GradientAlphaKey[_currentColorKeys.Length];

        //    for (int i = 0; i < _newColorKeys.Length; i++)
        //    {
        //        _newColorKeys[i].color = Color.Lerp(_currentColorKeys[i].color, deathFadeColorKeys[i].color, _evaluatedTimeValue);
        //    }

        //    for (int i = 0; i < _newAlphaKeys.Length; i++)
        //    {
        //        _newAlphaKeys[i].alpha = Mathf.Lerp(_currentAlphaKeys[i].alpha, deathFadeAlphaKeys[i].alpha, _evaluatedTimeValue);
        //    }

        //    Debug.Log("before: " + _newGradient);

        //    _newGradient.SetKeys(_newColorKeys, _newAlphaKeys);

        //    Debug.Log("after: " + _newGradient);

        //    vE.SetGradient("Gradient", _newGradient);

        //    yield return null;
        //}

        var _currentColor = visualEffect.GetVector4("HDRColorHSV");
        float _fadeTimeValue = 0;

        while (_fadeTimeValue < 1) {
            _fadeTimeValue += Time.deltaTime / fadeDuration;
            var _evaluatedTimeValue = deathFadeAnimCurve.Evaluate(_fadeTimeValue);
            var _newColor = Vector4.Lerp(_currentColor, fadeColor, _evaluatedTimeValue);

            visualEffect.SetVector4("HDRColorHSV", _newColor);

            yield return null;
        }

        deathAnimRoutine = null;

        Destroy(gameObject);

        yield return null;
    }
}

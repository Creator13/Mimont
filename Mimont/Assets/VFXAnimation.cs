using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using Util;

public class VFXAnimation : MonoBehaviour
{
    [Header("Death Animation")]
    [SerializeField] private float deathMinSizeAnimDuration;
    [SerializeField] private AnimationCurve deathMinSizeAnimCurve;
    [SerializeField] private float deathMaxSizeAnimDuration;
    [SerializeField] private AnimationCurve deathMaxSizeAnimCurve;
    [SerializeField] private float deathFadeAnimDuration;
    [SerializeField] private AnimationCurve deathFadeAnimCurve;
    [SerializeField] private float minDeathSize;
    [SerializeField] private float maxDeathSize;
    [SerializeField] private float deathFadeCount;
    //[SerializeField, GradientUsage(true)] private UnityEngine.Gradient deathFadeGradient;
    //private UnityEngine.GradientColorKey[] deathFadeColorKeys;
    //private UnityEngine.GradientAlphaKey[] deathFadeAlphaKeys;
    private Coroutine deathAnimRoutine;

    private VisualEffect vE;


    // Start is called before the first frame update
    void Start()
    {
        vE = GetComponent<VisualEffect>();

        //deathFadeColorKeys = deathFadeGradient.colorKeys;
        //deathFadeAlphaKeys = deathFadeGradient.alphaKeys;
    }

    private void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            PlayDeathAnim();
        }
    }

    public void PlayDeathAnim()
    {
        if(deathAnimRoutine != null) { StopCoroutine(deathAnimRoutine); }
        deathAnimRoutine = StartCoroutine(DeathAnimIE());
    }

    private IEnumerator DeathAnimIE()
    {
        float _currentSize = vE.GetFloat("Size");
        float _minTimeValue = 0;

        while (_minTimeValue < 1)
        {
            _minTimeValue += Time.deltaTime / deathMinSizeAnimDuration;
            float _evaluatedTimeValue = deathMinSizeAnimCurve.Evaluate(_minTimeValue);
            float _newSize = Mathf.Lerp(_currentSize, minDeathSize, _evaluatedTimeValue);

            vE.SetFloat("Size", _newSize);

            yield return null;
        }

        _currentSize = vE.GetFloat("Size");
        float _maxTimeValue = 0;

        while (_maxTimeValue < 1)
        {
            _maxTimeValue += Time.deltaTime / deathMaxSizeAnimDuration;
            float _evaluatedTimeValue = deathMaxSizeAnimCurve.Evaluate(_minTimeValue);
            float _newSize = Mathf.Lerp(_currentSize, maxDeathSize, _evaluatedTimeValue);

            vE.SetFloat("Size", _newSize);

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

        float _currentCount = vE.GetFloat("ParticleCount");
        float _fadeTimeValue = 0;

        while (_fadeTimeValue < 1)
        {
            _maxTimeValue += Time.deltaTime / deathMaxSizeAnimDuration;
            float _evaluatedTimeValue = deathFadeAnimCurve.Evaluate(_fadeTimeValue);
            float _newCount = Mathf.Lerp(_currentCount, deathFadeCount, _evaluatedTimeValue);

            vE.SetFloat("ParticleCount", _newCount);

            yield return null;
        }

        deathAnimRoutine = null;

        Destroy(this.gameObject);

        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class TargetVisualController : MonoBehaviour {
    [Header("Main Options")] [ColorUsage(true, true)] public Color targetColor;
    public float startScale;
    public float maxScale;
    public float addedScale;

    [Header("Growth Options")] [SerializeField] private int burstOutCount;
    [SerializeField] private float burstDelay;
    [SerializeField] private AnimationCurve growCurve;
    [SerializeField] private float growDuration;

    [Header("Spawn Options")] [SerializeField] private float orbFadeInDuration;
    [SerializeField] private AnimationCurve orbFadeInCurve;

    [Header("Hit Options")] [SerializeField] private float orbFadeGrowDuration;
    [SerializeField] private float orbFadeShrinkDuration;
    [SerializeField] private AnimationCurve orbFadeOutCurve;
    [SerializeField] private AnimationCurve orbFadeGrowCurve;
    [SerializeField] private AnimationCurve orbFadeShrinkCurve;

    [Header("Needed")] [SerializeField] private VisualEffect burstOut;
    [SerializeField] private VisualEffect burstIn;
    [SerializeField] private VisualEffect constant;

    private Material targetMat;
    private bool growCooldown = false;

    private Coroutine spawnRoutine;
    private Coroutine growRoutine;
    private Coroutine hitRoutine;

    void Start() {
        targetMat = GetComponent<Renderer>().material;

        //Set color
        targetMat.SetColor("_FresnelColor", targetColor);
        burstOut.SetVector4("Color", targetColor);
        burstIn.SetVector4("Color", targetColor);
        constant.SetVector4("Color", targetColor);

        //Idk waarom dit nodig is, maar ja fucking VFX graph
        burstOut.Stop();
        burstIn.Stop();
        constant.Stop();
        constant.enabled = false;
        burstIn.enabled = false;
        burstOut.enabled = false;
    }

    void Update() {
        //if (Keyboard.current.bKey.wasPressedThisFrame)
        //{
        //    StartSpawn();
        //}

        //if (Keyboard.current.nKey.wasPressedThisFrame)
        //{
        //    StartGrow();
        //}

        //if (Keyboard.current.mKey.wasPressedThisFrame)
        //{
        //    StartHit();
        //}
    }


    #region Spawn

    public void StartSpawn() {
        if (spawnRoutine != null) {
            return;
        }

        spawnRoutine = StartCoroutine(SpawnIE());
    }

    private IEnumerator SpawnIE() {
        constant.enabled = true;
        burstIn.enabled = true;
        burstOut.enabled = true;
        transform.localScale = new Vector3(startScale, startScale, startScale);

        burstOut.SetFloat("Count", 12f);

        constant.Play();
        burstOut.Play();
        yield return new WaitForSeconds(burstDelay);

        float _timeValue = 0;

        while (_timeValue < 1) {
            _timeValue += Time.deltaTime / orbFadeInDuration;
            float _evaluatedTimeValue = orbFadeInCurve.Evaluate(_timeValue);
            float _newOpacity = Mathf.Lerp(0f, 1f, _evaluatedTimeValue);

            targetMat.SetFloat("_Opacity", _newOpacity);

            yield return null;
        }

        spawnRoutine = null;
        yield return null;
    }

    #endregion


    #region Grow

    public void StartGrow() {
        if (growCooldown) {
            return;
        }

        growCooldown = true;
        StartCoroutine(GrowCooldownIE());
        StartCoroutine(GrowIE());
    }

    private IEnumerator GrowIE() {
        burstOut.SetFloat("Count", burstOutCount);
        burstOut.SetFloat("LifetimeTrail", 1.1f);
        burstOut.SetFloat("Size", 0.03f);
        burstOut.Play();
        yield return new WaitForSeconds(burstDelay);

        if (transform.localScale.x < maxScale) {
            float _timeValue = 0;

            Vector3 _oldSize = transform.localScale;
            Vector3 _newSize = transform.localScale + new Vector3(addedScale, addedScale, addedScale);

            while (_timeValue < 1) {
                _timeValue += Time.deltaTime / growDuration;
                float _evaluatedTimeValue = growCurve.Evaluate(_timeValue);
                Vector3 _Size = Vector3.Lerp(_oldSize, _newSize, _evaluatedTimeValue);

                transform.localScale = _Size;

                yield return null;
            }
        }
        else {
            GetComponent<Renderer>().enabled = false;
            constant.Stop();
            //disable collider
            yield return new WaitForSeconds(4f);
            Destroy(gameObject);
        }

        growRoutine = null;

        yield return null;
    }

    private IEnumerator GrowCooldownIE() {
        yield return new WaitForSeconds(growDuration);

        growCooldown = false;
    }

    #endregion


    #region Hit

    public void StartHit() {
        if (hitRoutine != null) {
            return;
        }

        hitRoutine = StartCoroutine(HitIE());
    }

    private IEnumerator HitIE() {
        float _growTimeValue = 0;

        float _startScale = targetMat.GetFloat("_ScaleOffset");

        while (_growTimeValue < 1) {
            _growTimeValue += Time.deltaTime / orbFadeGrowDuration;
            float _evaluatedTimeValue = orbFadeGrowCurve.Evaluate(_growTimeValue);
            float _newScale = Mathf.Lerp(_startScale, _startScale + 0.1f, _evaluatedTimeValue);

            targetMat.SetFloat("_ScaleOffset", _newScale);

            yield return null;
        }

        float _shrinkTimeValue = 0;

        burstIn.Play();

        while (_shrinkTimeValue < 1) {
            _shrinkTimeValue += Time.deltaTime / orbFadeShrinkDuration;
            float _evaluatedTimeShrink = orbFadeShrinkCurve.Evaluate(_shrinkTimeValue);
            float _evaluatedTimeOpacity = orbFadeOutCurve.Evaluate(_shrinkTimeValue);
            float _newScale = Mathf.Lerp(_startScale + 0.1f, _startScale - 0.05f, _evaluatedTimeShrink);
            targetMat.SetFloat("_ScaleOffset", _newScale);
            float _newOpacity = Mathf.Lerp(1f, 0f, _evaluatedTimeOpacity);
            targetMat.SetFloat("_Opacity", _newOpacity);

            yield return null;
        }

        constant.Stop();

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);

        hitRoutine = null;
        yield return null;
    }

    #endregion
}

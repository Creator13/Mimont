using System.Collections;
using Hellmade.Sound;
using Mimont;
using UnityEngine;
using UnityEngine.VFX;

public class TargetVisualController : MonoBehaviour {
    private static readonly int NoiseOffset = Shader.PropertyToID("_NoiseOffset");
    private static readonly int ScrollDirection = Shader.PropertyToID("_ScrollDirection");
    private static readonly int FresnelColor = Shader.PropertyToID("_FresnelColor");
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");
    private static readonly int ScaleOffset = Shader.PropertyToID("_ScaleOffset");

    [Header("Main Options")] [ColorUsage(true, true)] public Color targetColor;
    public Vector3 startScale;
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

    [Header("Sound effects")] 
    [SerializeField] private AudioClip grow1;
    [SerializeField] private AudioClip grow2;

    private static bool playingGrow1;
    private static bool playingGrow2;
    
    private Material targetMat;
    private bool growCooldown = false;

    private Coroutine spawnRoutine;
    private Coroutine growRoutine;
    private Coroutine hitRoutine;

    private void Awake() {
        targetMat = GetComponent<Renderer>().material;
    }

    private void Start() {
        //Set color
        targetMat.SetColor(FresnelColor, targetColor);
        burstOut.SetVector4("Color", targetColor);
        burstIn.SetVector4("Color", targetColor);
        constant.SetVector4("Color", targetColor);

        MimontGame.OnBeat += StartGrow;
    }

    private void OnDisable() {
        MimontGame.OnBeat -= StartGrow;
    }


    #region Spawn

    public void StartSpawn() {
        if (spawnRoutine != null) {
            return;
        }

        spawnRoutine = StartCoroutine(SpawnIE());
    }

    private IEnumerator SpawnIE() {
        targetMat.SetVector(NoiseOffset, new Vector2(Random.Range(0, 1000), Random.Range(0, 1000)));
        targetMat.SetFloat(ScrollDirection, Random.Range(0, 360));

        constant.enabled = true;
        burstIn.enabled = true;
        burstOut.enabled = true;
        transform.localScale = startScale;

        burstOut.SetFloat("Count", 12f);

        constant.SendEvent("OnPlayyy");
        burstOut.SendEvent("OnPlayyy");
        yield return new WaitForSeconds(burstDelay);

        float _timeValue = 0;

        while (_timeValue < 1) {
            _timeValue += Time.deltaTime / orbFadeInDuration;
            var _evaluatedTimeValue = orbFadeInCurve.Evaluate(_timeValue);
            var _newOpacity = Mathf.Lerp(0f, 1f, _evaluatedTimeValue);

            targetMat.SetFloat(Opacity, _newOpacity);

            yield return null;
        }

        spawnRoutine = null;
    }

    #endregion


    #region Grow

    public void StartGrow() {
        // Debug.Log($"Starting grow, {hitRoutine}");
        if (spawnRoutine != null) return;
        if (growRoutine != null) return;
        if (hitRoutine != null) return;
        if (growCooldown) return;

        growCooldown = true;
        growRoutine = StartCoroutine(GrowCooldownIE());
        StartCoroutine(GrowIE());
    }

    private IEnumerator GrowIE() {
        burstOut.SetFloat("Count", burstOutCount);
        burstOut.SetFloat("LifetimeTrail", 1.1f);
        burstOut.SetFloat("Size", 0.03f);
        if (!playingGrow1) {
            playingGrow1 = true;
            EazySoundManager.PlaySound(grow1);
        }
        burstOut.SendEvent("OnPlayyy");
        yield return new WaitForSeconds(burstDelay);

        if (transform.localScale.x < maxScale) {
            float _timeValue = 0;

            var _oldSize = transform.localScale;
            var _newSize = transform.localScale + new Vector3(addedScale, addedScale, addedScale);

            if (!playingGrow2) {
                playingGrow2 = true;
                EazySoundManager.PlaySound(grow2);
            }
            while (_timeValue < 1) {
                _timeValue += Time.deltaTime / growDuration;
                var _evaluatedTimeValue = growCurve.Evaluate(_timeValue);
                var _size = Vector3.Lerp(_oldSize, _newSize, _evaluatedTimeValue);

                transform.localScale = _size;

                yield return null;
            }
        }
        else {
            GetComponent<Renderer>().enabled = false;
            constant.SendEvent("OnStoppp");
            //disable collider
            yield return new WaitForSeconds(4f);
            Kill();
        }

        playingGrow1 = false;
        playingGrow2 = false;
        growRoutine = null;
    }

    private IEnumerator GrowCooldownIE() {
        yield return new WaitForSeconds(growDuration);

        growCooldown = false;
    }

    #endregion


    #region Hit

    public void StartHit(System.Action callback) {
        if (spawnRoutine != null) return;
        if (hitRoutine != null) return;

        hitRoutine = StartCoroutine(HitIE(callback));
    }

    private IEnumerator HitIE(System.Action callback) {
        float _growTimeValue = 0;

        var _startScale = targetMat.GetFloat(ScaleOffset);

        while (_growTimeValue < 1) {
            _growTimeValue += Time.deltaTime / orbFadeGrowDuration;
            var _evaluatedTimeValue = orbFadeGrowCurve.Evaluate(_growTimeValue);
            var _newScale = Mathf.Lerp(_startScale, _startScale + 0.1f, _evaluatedTimeValue);

            
            
            targetMat.SetFloat(ScaleOffset, _newScale);

            yield return null;
        }

        float _shrinkTimeValue = 0;

        burstIn.SendEvent("OnPlayyy");

        while (_shrinkTimeValue < 1) {
            _shrinkTimeValue += Time.deltaTime / orbFadeShrinkDuration;
            var _evaluatedTimeShrink = orbFadeShrinkCurve.Evaluate(_shrinkTimeValue);
            var _evaluatedTimeOpacity = orbFadeOutCurve.Evaluate(_shrinkTimeValue);
            var _newScale = Mathf.Lerp(_startScale + 0.1f, _startScale - 0.05f, _evaluatedTimeShrink);
            targetMat.SetFloat(ScaleOffset, _newScale);
            var _newOpacity = Mathf.Lerp(1f, 0f, _evaluatedTimeOpacity);
            targetMat.SetFloat(Opacity, _newOpacity);

            yield return null;
        }

        constant.SendEvent("OnStoppp");

        callback();

        yield return new WaitForSeconds(2.5f);

        Kill();

        hitRoutine = null;
    }

    #endregion


    private void Kill() {
        MimontGame.OnBeat -= StartGrow;
        Destroy(gameObject);
    }
}

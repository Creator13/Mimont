using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class TargetVisualController : MonoBehaviour
{
    [SerializeField] private float VFXdelay;
    [SerializeField] private float addedScale;
    [SerializeField] private AnimationCurve growCurve;
    [SerializeField] private float growDuration;
    [SerializeField, ColorUsage(true, true)] private Color targetColor;
    [Space]
    [SerializeField] private VisualEffect burst;
    [SerializeField] private VisualEffect constant;
    private Material targetMat;

    private Coroutine growRoutine;

    // Start is called before the first frame update
    void Start()
    {
        targetMat = GetComponent<Renderer>().material;

        targetMat.SetColor("_FresnelColor", targetColor);
        burst.SetVector4("Color", targetColor);
        constant.SetVector4("Color", targetColor);
        burst.Stop();
        constant.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            StartGrow();
        }
    }

    private void StartGrow()
    {
        if(growRoutine != null) { return; }
        growRoutine = StartCoroutine(GrowIE());
    }

    private IEnumerator GrowIE()
    {
        burst.Play();
        yield return new WaitForSeconds(VFXdelay);

        float _timeValue = 0;

        Vector3 _oldSize = transform.localScale;
        Vector3 _newSize = transform.localScale + new Vector3(addedScale, addedScale, addedScale);

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / growDuration;
            float _evaluatedTimeValue = growCurve.Evaluate(_timeValue);
            Vector3 _Size = Vector3.Lerp(_oldSize, _newSize, _evaluatedTimeValue);

            transform.localScale = _Size;

            yield return null;
        }


        growRoutine = null;
        yield return null;
    }
}

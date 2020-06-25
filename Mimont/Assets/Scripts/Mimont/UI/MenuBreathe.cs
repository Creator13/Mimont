using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuBreathe : MonoBehaviour
{
    [SerializeField] private float minBloom;
    [SerializeField] private float maxBloom;
    [SerializeField] private float baseBloom;
    [SerializeField] private float breatheSpeed;

    private VolumeProfile v;
    private Bloom b;


    // Start is called before the first frame update
    void Start()
    {
        v = GameObject.FindObjectOfType<Volume>().profile;
        v.TryGet(out b);
    }

    // Update is called once per frame
    void Update()
    {
        b.intensity.value = Mathf.Lerp(minBloom, maxBloom, Mathf.Sin(Time.time * breatheSpeed));
    }

    public void ResetBloom()
    {
        b.intensity.value = baseBloom;
    }
}

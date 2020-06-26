﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class MenuBreathe : MonoBehaviour
{
    [Header("Menu Breathing")]
    [SerializeField] private float minBloom;
    [SerializeField] private float maxBloom;
    [SerializeField] private float baseBloom;
    [SerializeField] private float breatheSpeed;
    [Header("Highlight")]
    [SerializeField] private float highlightIntensity;
    [Header("Random Color")]
    [SerializeField, ColorUsage(true, true)] private Color[] colors;
    [SerializeField] private  float intensityValue;
    [SerializeField] private Material[] spriteMats;
    [SerializeField] private Material[] TMProMats;

    private VolumeProfile v;
    private Bloom b;


    // Start is called before the first frame update
    void Start()
    {
        v = GameObject.FindObjectOfType<Volume>().profile;
        v.TryGet(out b);

        //Shuffle
        for (int i = 0; i < colors.Length; i++)
        {
            int rnd = Random.Range(0, colors.Length);
            Color tempColor = colors[rnd];
            colors[rnd] = colors[i];
            colors[i] = tempColor;
        }

        for (int i = 0; i < spriteMats.Length; i++)
        {
            spriteMats[i].SetColor("_Color", colors[i]);
        }

        for (int i = 0; i < TMProMats.Length; i++)
        {
            //TMProMats[i].SetColor("_FaceColor", ColToTmpCol(colors[spriteMats.Length]));
            TMProMats[i].SetColor("_FaceColor", colors[spriteMats.Length]);
        }
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

    private Color TextDilationCol(Color col)
    {
        Color newCol = new Vector4(col.r * (Mathf.Pow(2f, 1f)), col.g * (Mathf.Pow(2f, 1f)), col.b * (Mathf.Pow(2f, 1f)), col.a);
        return newCol;
    }

    public void HighlightSprite(Renderer r)
    {
        Material mat = r.material;
        Vector4 oldCol = mat.GetColor("_Color");
        Vector4 newCol = oldCol * (Mathf.Pow(2, highlightIntensity));
        mat.SetColor("_Color", newCol);
    }

    public void ResetSprite(Renderer r)
    {
        Material mat = r.material;
        Vector4 oldCol = mat.GetColor("_Color");
        Vector4 newCol = oldCol / (Mathf.Pow(2, highlightIntensity));
        mat.SetColor("_Color", newCol);
    }

    public void HighlightTMPro(Material mat)
    {
        Vector4 oldCol = mat.GetColor("_FaceColor");
        Vector4 newCol = oldCol * (Mathf.Pow(2, highlightIntensity));
        mat.SetColor("_FaceColor", newCol);
    }

    public void ResetTMPro(Material mat)
    {
        Vector4 oldCol = mat.GetColor("_FaceColor");
        Vector4 newCol = oldCol / (Mathf.Pow(2, highlightIntensity));
        mat.SetColor("_FaceColor", newCol);
    }
}

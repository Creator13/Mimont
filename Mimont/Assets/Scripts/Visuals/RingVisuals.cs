using System;
using System.Collections.Generic;
using Mimont.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

public class RingVisuals : MonoBehaviour {
    private static readonly int ScaleOffset = Shader.PropertyToID("_ScaleOffset");
    private static readonly int LineWidth = Shader.PropertyToID("_LineWidth");
    private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");
    private static readonly int DisplacementScale = Shader.PropertyToID("_DisplacementScale");
    private static readonly int DisplacementNoiseFrequency = Shader.PropertyToID("_DisplacementNoiseFrequency");
    private static readonly int DisplacementScrollDirection = Shader.PropertyToID("_DisplacementScrollDirection");
    private static readonly int DisplacementNoiseOffset = Shader.PropertyToID("_DisplacementNoiseOffset");
    private static readonly int LineColor = Shader.PropertyToID("_LineColor");

    [SerializeField] private Transform icospherePrefab;
    [SerializeField] private int lineAmount = 3;
    [SerializeField] private float lineWidth = .8f;
    [SerializeField] private float scale;
    [SerializeField] private float scaleOffset = -.04f;
    [SerializeField] private float scrollSpeed = .3f;
    [SerializeField] private float displacementScale = .08f;
    [SerializeField] private float noiseFrequency = 3;
    [SerializeField] private Material lineMaterial;

    public SpherePool icospherePool;
    public Color[] startColors;
    private List<Transform> Lines { get; } = new List<Transform>();
    private int updatedColor;

    public void Spawn(Vector3 spawnLocation) {
        updatedColor = 0;

        for (var i = 0; i < lineAmount; i++) {
            var scrollDirection = (360f / lineAmount) * i;
            var noiseOffset = (1000f / lineAmount) * i;

            var newLine = GetSphere();
            newLine.transform.position = spawnLocation;

            var lineMat = CreateMaterial(scrollDirection, noiseOffset, startColors[i % startColors.Length]);
            var r = newLine.GetComponent<Renderer>();
            r.material = lineMat;

            newLine.transform.localScale = Vector3.one;
            Lines.Add(newLine);
        }
    }

    public void UpdateColor(Color newColor) {
        if (updatedColor < Lines.Count) {
            var m = new List<Material>();
            Lines[updatedColor].GetComponent<Renderer>().GetMaterials(m);
            foreach (var mat in m) {
                mat.SetColor(LineColor, newColor);
            }
        }
        else {
            var scrollDirection = Random.Range(0f, 360f);
            var noiseOffset = Random.Range(0f, 1000f);

            var newLine = GetSphere();

            var r = newLine.GetComponent<Renderer>();
            r.enabled = false;
            newLine.transform.localScale = new Vector3(scale, scale, 1);

            var lineMat = CreateMaterial(scrollDirection, noiseOffset, newColor);

            r.material = lineMat;
            r.enabled = true;
            Lines.Add(newLine);
        }

        updatedColor++;
    }

    public void UpdateRadius(float newRadius) {
        var newScale = new Vector3(newRadius * 2, newRadius * 2, newRadius);

        foreach (var line in Lines) {
            line.transform.localScale = newScale;
        }
    }

    public void Destroy() {
        foreach (var line in Lines) {
            ReturnSpere(line);
        }

        Lines.Clear();
    }

    private Material CreateMaterial(float scrollDirection, float noiseOffset, Color color) {
        var mat = new Material(lineMaterial);
        mat.SetFloat(ScaleOffset, scaleOffset);
        mat.SetFloat(LineWidth, lineWidth);
        mat.SetFloat(ScrollSpeed, scrollSpeed);
        mat.SetFloat(DisplacementScale, displacementScale);
        mat.SetFloat(DisplacementNoiseFrequency, noiseFrequency);

        mat.SetFloat(DisplacementScrollDirection, scrollDirection);
        mat.SetFloat(DisplacementNoiseOffset, noiseOffset);
        mat.SetColor(LineColor, color);

        return mat;
    }

    private Transform GetSphere() {
        var sphere = icospherePool.Get();
        sphere.transform.localRotation = Quaternion.identity;
        sphere.localScale = Vector3.zero;
        sphere.SetParent(transform, false);
        DestroyImmediate(sphere.GetComponent<MeshRenderer>());
        sphere.gameObject.AddComponent<MeshRenderer>();
        sphere.gameObject.SetActive(true);
        
        return sphere;
    }

    private void ReturnSpere(Transform sphere) {
        icospherePool.ReturnObject(sphere);
    }
}

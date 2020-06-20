using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KissVisualsManager : MonoBehaviour
{
    [SerializeField] private GameObject icoSphere;
    [SerializeField] private int lineAmount;
    [SerializeField] private float lineWidth;
    [SerializeField] private float scale;
    [SerializeField] private float scaleOffset;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float displacementScale;
    [SerializeField] private float noiseFrequency;

    [SerializeField] private Color[] colors;
    private Shader lineShader;
    private List<GameObject> lines = new List<GameObject>();
    private int updatedColor = 0;


    void Start()
    {
        updatedColor = 0;
        lineShader = Shader.Find("Custom/KissGraph");
        Spawn(Vector3.zero);
    }

    public void Spawn(Vector3 spawnLocation)
    {
        for (int i = 0; i < lineAmount; i++)
        {
            float scrollDirection = (360f / lineAmount) * i;
            float noiseOffset = (1000f / lineAmount) * i;

            GameObject newLine = Instantiate(icoSphere, spawnLocation, Quaternion.identity, transform);
            Renderer r = newLine.GetComponent<Renderer>();

            Material lineMat = new Material(lineShader);
            lineMat.SetFloat("_ScaleOffset", scaleOffset);
            lineMat.SetFloat("_LineWidth", lineWidth);
            lineMat.SetFloat("_ScrollSpeed", scrollSpeed);
            lineMat.SetFloat("_DisplacementScale", displacementScale);
            lineMat.SetFloat("_DisplacementNoiseFrequency", noiseFrequency);

            lineMat.SetFloat("_DisplacementScrollDirection", scrollDirection);
            lineMat.SetFloat("_DisplacementNoiseOffset", noiseOffset);
            //lineMat.SetColor("_LineColor", colors[colors.Length - (colors.Length % (i + 1))]);
            //Debug.Log(i % colors.Length);
            lineMat.SetColor("_LineColor", colors[i % colors.Length]);

            r.material = lineMat;

            //newLine.transform.localScale = Vector3.one * scale;
            newLine.transform.localScale = Vector3.one;
            lines.Add(newLine);
        }
    }

    public void UpdateColor(Color newColor)
    {
        if(updatedColor < lines.Count)
        {
            List<Material> m = new List<Material>();
            lines[updatedColor].GetComponent<Renderer>().GetMaterials(m);
            for (int i = 0; i < m.Count; i++)
            {
                m[i].SetColor("_LineColor", newColor);
            }
        }
        else
        {
            float scrollDirection = Random.Range(0f, 360f);
            float noiseOffset = Random.Range(0f, 1000f);

            GameObject newLine = Instantiate(icoSphere, Vector3.zero, Quaternion.identity, transform);
            Renderer r = newLine.GetComponent<Renderer>();
            r.enabled = false;
            newLine.transform.localScale = new Vector3(scale, scale, 1);

            Material lineMat = new Material(lineShader);
            lineMat.SetFloat("_ScaleOffset", scaleOffset);
            lineMat.SetFloat("_LineWidth", lineWidth);
            lineMat.SetFloat("_ScrollSpeed", scrollSpeed);
            lineMat.SetFloat("_DisplacementScale", displacementScale);
            lineMat.SetFloat("_DisplacementNoiseFrequency", noiseFrequency);

            lineMat.SetFloat("_DisplacementScrollDirection", scrollDirection);
            lineMat.SetFloat("_DisplacementNoiseOffset", noiseOffset);
            //lineMat.SetColor("_LineColor", colors[colors.Length - (colors.Length % (i + 1))]);
            //Debug.Log(i % colors.Length);
            lineMat.SetColor("_LineColor", newColor);

            r.material = lineMat;

            r.enabled = true;

            lines.Add(newLine);
        }
        updatedColor++;
    }

    public void UpdateScale(Vector3 newScale)
    {
        newScale.z = 1;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].transform.localScale = newScale;
        }
    }

    public void Destroy()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Destroy(lines[i].gameObject);
        }
    }

    void Update()
    {
        //UpdateScale(scale);

        //if (Keyboard.current.bKey.isPressed)
        //{
        //    scale += Time.deltaTime/4f;
        //}
        //if (Keyboard.current.lKey.wasPressedThisFrame)
        //{
        //    UpdateColor(Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f));
        //}
        //if (Keyboard.current.pKey.wasPressedThisFrame)
        //{
        //    Spawn(Vector3.zero);
        //}
        //if (Keyboard.current.oKey.wasPressedThisFrame)
        //{
        //    Destroy();
        //}
    }
}

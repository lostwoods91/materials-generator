using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

class MaterialDefinition
{
    public MaterialDefinition()
    {
        verbose = false;
    }

    public void setVerbose(bool value)
    {
        verbose = value;
    }

    private bool isValid()
    {
        return name != null;
    }

    private void reset()
    {
        name = null;
        baseColor = Color.white;
        baseTexture = null;
        metallic = 0.0f;
        roughness = 0.0f;
        emissiveColor = Color.black;
        //opacity = 1.0f;        // integrated in baseColor
        normalTexture = null;
        heightFieldTexture = null;
    }

    public void setName(string value)
    {
        name = value;
    }

    public void setBaseColor(float r, float g, float b)
    {
        baseColor.r = r;
        baseColor.g = g;
        baseColor.b = b;
    }

    public void setBaseTexture(string value)
    {
        baseTexture = value;
    }

    public void setMetallic(float value)
    {
        metallic = value;
    }

    public void setRoughness(float value)
    {
        roughness = value;
    }

    public void setEmissiveColor(float r, float g, float b)
    {
        emissiveColor.r = r;
        emissiveColor.g = g;
        emissiveColor.b = b;
    }

    public void setOpacity(float value)
    {
        baseColor.a = value;
    }

    public void setNormalTexture(string value)
    {
        normalTexture = value;
    }

    public void setHeightFieldTexture(string value)
    {
        heightFieldTexture = value;
    }

    public void create()
    {
        if (isValid())
        {
            // mi assicuro che non esista già
            Assert.AreEqual(Directory.GetFiles(customMaterialsFolder, name + ".mat").Length, 0);

            // compongo i parametri
            Material material = new Material(Shader.Find("Standard"));

            material.color = baseColor;

            Texture2D baseText = Resources.Load<Texture2D>(modelName + "/" + baseTexture.Split('.')[0]);
            if (baseText != null) material.mainTexture = baseText;

            material.SetFloat(Shader.PropertyToID("_Metallic"), metallic);

            material.SetFloat(Shader.PropertyToID("_Glossiness"), 1.0f - roughness);

            material.SetColor(Shader.PropertyToID("_EmissionColor"), emissiveColor);

            if (baseColor.a < 1.0f)
            {
                material.SetFloat(Shader.PropertyToID("_Mode"), 3);
            }

            Texture2D normalText = Resources.Load<Texture2D>(modelName + "/" + normalTexture.Split('.')[0]);
            if (normalText != null) material.SetTexture(Shader.PropertyToID("_BumpMap"), normalText);

            Texture2D heightFieldText = Resources.Load<Texture2D>(modelName + "/" + heightFieldTexture.Split('.')[0]);
            if (heightFieldText != null) material.SetTexture(Shader.PropertyToID("_ParallaxMap"), heightFieldText);

            // quindi lo creo
            string materialPath = customMaterialsFolder + "/" + name + ".mat";
            AssetDatabase.CreateAsset(material, materialPath);

            if (verbose)
            {
                Debug.Log("Created material:\t" + name + "\n");
            }
        }
        // annullo la validità del materiale appena analizzato
        reset();
    }

    public static void setBaseMaterialsFolder(string value)
    {
        baseMaterialsFolder = value;
    }

    public static void setCustomMaterialsFolder(string value)
    {
        customMaterialsFolder = value;
    }

    public static void setModelName(string value)
    {
        modelName = value;
    }

    private static string baseMaterialsFolder = null;
    private static string customMaterialsFolder = null;
    private static string modelName = null;

    private string name = null;
    private Color baseColor = Color.white;
    private string baseTexture = null;
    private float metallic = 0.0f;
    private float roughness = 0.0f;
    private Color emissiveColor = Color.black;
    //private float opacity;        // integrated in baseColor
    private string normalTexture = null;
    private string heightFieldTexture = null;

    private bool valid = false;
    private bool verbose = false;
}
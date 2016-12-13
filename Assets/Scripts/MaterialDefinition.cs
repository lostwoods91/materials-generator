using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

class MaterialDefinition
{
    public MaterialDefinition()
    {
        setInvalid();
        verbose = true;
    }

    public void setVerbose(bool value)
    {
        verbose = value;
    }

    private void setInvalid()
    {
        name = null;
        basemat = null;
        texture = null;
        hasColor = false;
    }

    private bool isValid()
    {
        return name != null && basemat != null;
    }

    public void setName(string value)
    {
        name = value;
    }

    public void setBaseMat(string value)
    {
        basemat = value;
    }

    public void setTexture(string value)
    {
        texture = value;
    }

    public void setColor(string value)
    {
        // TODO not implemented yet
        hasColor = true;
    }

    public void create()
    {
        if (isValid())
        {
            if (texture == null && !hasColor)
            {
                // è un materiale di base, mi assicuro che esista
                Assert.AreNotEqual(Directory.GetFiles(baseMaterialsFolder, basemat + ".mat").Length, 0);

                if (verbose)
                {
                    Debug.Log("Found base material:\t" + name + "\n");
                }
            }
            else
            {
                // è un materiale custom, mi assicuro che non esista già
                Assert.AreEqual(Directory.GetFiles(customMaterialsFolder, name + ".mat").Length, 0);

                // quindi lo creo
                string materialPath = customMaterialsFolder + "/" + name + ".mat";
                Material material;
                Material temp = (Material)Resources.Load(basemat);
                if (temp != null)
                {
                    material = new Material(temp);
                }
                else
                {
                    material = new Material(Shader.Find("Standard"));
                }
                if (texture != null)
                {
                    Texture2D text = Resources.Load<Texture2D>(modelName + "/" + texture.Split('.')[0]);
                    material.mainTexture = text;
                }
                if (hasColor)
                {
                    // TODO not implemented yet
                }
                AssetDatabase.CreateAsset(material, materialPath);

                if (verbose)
                {
                    Debug.Log("Created material:\t" + name + "\n");
                    Debug.Log("\t- with base material:\t" + basemat + "\n");
                    if (texture != null) Debug.Log("\t- with texture:\t" + texture + "\n");
                    if (hasColor) Debug.Log("\t- with color:\t" + color + "\n");
                }
            }
        }
        // annullo la validità del materiale appena analizzato
        setInvalid();
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

    private string name;
    private string basemat;
    private string texture;
    private bool hasColor;
    private Color color;

    private bool verbose;
}
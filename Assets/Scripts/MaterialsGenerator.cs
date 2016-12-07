using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;
using UnityEngine.Assertions;

public class MaterialsGenerator : MonoBehaviour
{
    void Start()
    {
        if (searchForUtmFile())
        {
            initFolder();
            generateCustomMaterials();
        }

    }

    private bool searchForUtmFile()
    {
        string[] found = Directory.GetFiles(modelFolderPath, "*.utm");
        if (found.Length > 0)
        {
            // la prima occorrenza contiene il percorso completo, prendo solo il nome del file
            string[] tokens = found[0].Split('\\');
            utmFile = tokens[tokens.Length - 1];
            return true;
        }
        else
            return false;
    }

    private void initFolder()
    {
        customMaterialsFolder = modelFolderPath + "/" + customMaterialsFolderName;
        if (Directory.Exists(customMaterialsFolder))
        {
            Directory.Delete(customMaterialsFolder, true);
        }
        Directory.CreateDirectory(customMaterialsFolder);
        Debug.Log("initialized CustomMaterials folder: " + customMaterialsFolder);
    }

    private bool generateCustomMaterials()
    {
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file was saved as
            StreamReader theReader = new StreamReader(modelFolderPath + "/" + utmFile, Encoding.Default);

            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // inizializzo la definizione di materiale
                current = new MaterialDefinition();
                //current.setVerbose(false);
                MaterialDefinition.setModelFolder(modelFolderPath);
                MaterialDefinition.setCustomMaterialsFolder(customMaterialsFolder);
                MaterialDefinition.setBaseMaterialsFolder("Assets/" + baseMaterialsFolderName + "/Resources");

                // mi assicuro che venga generata un'eccezione se qualcosa dovesse andare storto durante il parsing
                //Assert.raiseExceptions = false;

                // leggo il file riga per riga
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        parseLine(line);
                    }
                }
                while (line != null);

                // se ho una definizione valida attiva, creo l'ultimo materiale
                current.create();

                // Done reading, close the reader and return true to broadcast success
                theReader.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    private void parseLine(string line)
    {
        if (line.Length < 2) return;

        if (line[0] == '#')
        {
            Debug.Log("commento:\n" + line);
            return;     // commento
        }

        string[] instruction = line.Split(' ');
        string command = instruction[0];
        string value = instruction[1];

        if (command == UTM_MATERIAL_DEFINITION)
        {
            current.create();
            current.setName(value);
            return;
        }

        if (command == UTM_SET_BASE_MATERIAL)
        {
            current.setBaseMat(value);
            return;
        }

        if (command == UTM_SET_TEXTURE)
        {
            current.setTexture(value);
            return;
        }

        if (command == UTM_SET_COLOR)
        {
            current.setColor(value);
            return;
        }
    }

    private const string customMaterialsFolderName = "CustomMaterials";
    private const string baseMaterialsFolderName = "UTesyMaterials";

    #region UTM_COMMANDS

    // definizione di un nuovo materiale
    private const string UTM_MATERIAL_DEFINITION = "definemat";

    // personalizzazioni materiale
    private const string UTM_SET_BASE_MATERIAL = "basemat";                                         // definizione della texture che il materiale personalizzato deve usare
    private const string UTM_SET_TEXTURE = "settext";                                               // definizione della texture che il materiale personalizzato deve usare
    private const string UTM_SET_COLOR = "setcolor";

    #endregion

    private string utmFile;
    private string customMaterialsFolder;
    private MaterialDefinition current;

    [SerializeField]
    private string modelFolderPath;

}

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
                //Assert.AreNotEqual(Directory.GetFiles(baseMaterialsFolder, basemat + ".mat").Length, 0);

                if (verbose)
                {
                    Debug.Log("Found base material:\t" + name + "\n");
                }
            }
            else
            {
                // è un materiale custom, mi assicuro che non esista già
                //Assert.AreNotEqual(Directory.GetFiles(customMaterialsFolder, name + ".mat").Length, 0);

                // quindi lo creo
                string materialPath = customMaterialsFolder + "/" + name + ".mat";
                //Material material = new Material(Shader.Find("Standard"));
                Material material = new Material((Material)Resources.Load(basemat));
                if (texture != null)
                {
                    Texture2D text = (Texture2D)Resources.Load(texture.Split('.')[0]);
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

    public static void setModelFolder(string value)
    {
        modelFolder = value;
    }

    private static string baseMaterialsFolder = null;
    private static string customMaterialsFolder = null;
    private static string modelFolder = null;

    private string name;
    private string basemat;
    private string texture;
    private bool hasColor;
    private Color color;

    private bool verbose;
}
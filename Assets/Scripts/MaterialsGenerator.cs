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
        customMaterialsFolder = modelFolderPath + "/" + customMaterialsFolderName;
        string[] tokens = modelFolderPath.Split('/');
        modelName = tokens[tokens.Length - 1];
        if (!Directory.Exists(customMaterialsFolder) || overwrite)
        {
            utmFile = modelFolderPath + "/" + modelName + ".utm";
            if (!Directory.Exists(utmFile))
            {
                initFolder();
                generateCustomMaterials();
            }
        }
    }

    //private bool searchForUtmFile()
    //{
    //    string[] found = Directory.GetFiles(modelName, "*.utm");
    //    if (found.Length > 0)
    //    {
    //        // la prima occorrenza contiene il percorso completo, prendo solo il nome del file
    //        string[] tokens = found[0].Split('\\');
    //        utmFile = tokens[tokens.Length - 1];
    //        return true;
    //    }
    //    else
    //        return false;
    //}

    private void initFolder()
    {
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
            StreamReader theReader = new StreamReader(utmFile, Encoding.Default);

            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // inizializzo la definizione di materiale
                current = new MaterialDefinition();
                current.setVerbose(true);
                MaterialDefinition.setModelName(modelName);
                MaterialDefinition.setCustomMaterialsFolder(customMaterialsFolder);
                MaterialDefinition.setBaseMaterialsFolder("Assets/" + baseMaterialsFolderName + "/Resources");

                // mi assicuro che venga generata un'eccezione se qualcosa dovesse andare storto durante il parsing
                Assert.raiseExceptions = true;

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

        string[] instruction = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        string command = instruction[0];
        string value = instruction[1];

        if (command == UTM_MATERIAL_DEFINITION)
        {
            current.create();
            current.setName(value);
            return;
        }

        if (command == UTM_BASE_MATERIAL_NAME)
        {
            return;
        }

        if (command == UTM_BASE_COLOR)
        {
            Assert.AreEqual(instruction.Length, 4);
            current.setBaseColor(Convert.ToSingle(instruction[1]), Convert.ToSingle(instruction[2]), Convert.ToSingle(instruction[3]));
            return;
        }

        if (command == UTM_BASE_TEXTURE)
        {
            current.setBaseTexture(value);
            return;
        }

        if (command == UTM_METALLIC)
        {
            current.setMetallic(Convert.ToSingle(value));
            return;
        }

        if (command == UTM_ROUGHNESS)
        {
            current.setRoughness(Convert.ToSingle(value));
            return;
        }

        if (command == UTM_EMISSIVE_COLOR)
        {
            Assert.AreEqual(instruction.Length, 4);
            current.setEmissiveColor(Convert.ToSingle(instruction[1]), Convert.ToSingle(instruction[2]), Convert.ToSingle(instruction[3]));
            return;
        }

        if (command == UTM_OPACITY)
        {
            current.setOpacity(Convert.ToSingle(value));
            return;
        }

        if (command == UTM_NORMAL_TEXTURE)
        {
            current.setNormalTexture(value);
            return;
        }

        if (command == UTM_HEIGHTFIELD_TEXTURE)
        {
            current.setHeightFieldTexture(value);
            return;
        }
    }

    private const string customMaterialsFolderName = "Materials";
    private const string baseMaterialsFolderName = "UTesyMaterials";

    #region UTM_COMMANDS

    // UTM spec
    const string UTM_MATERIAL_DEFINITION = "definemat";
    const string UTM_BASE_MATERIAL_NAME = "basename";
    const string UTM_BASE_COLOR = "basecolor";
    const string UTM_BASE_TEXTURE = "basetexture";
    const string UTM_METALLIC = "metallic";
    const string UTM_ROUGHNESS = "roughness";
    const string UTM_EMISSIVE_COLOR = "emissivecolor";
    const string UTM_OPACITY = "opacity";
    const string UTM_NORMAL_TEXTURE = "normal";
    const string UTM_HEIGHTFIELD_TEXTURE = "heightfield";

    #endregion

    private string modelName;
    private string utmFile;
    private string customMaterialsFolder;
    private MaterialDefinition current;

    [SerializeField]
    private string modelFolderPath;
    [SerializeField]
    private bool overwrite = false;
}
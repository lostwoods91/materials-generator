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
                //current.setVerbose(false);
                MaterialDefinition.setModelName(modelName);
                MaterialDefinition.setCustomMaterialsFolder(customMaterialsFolder);
                MaterialDefinition.setBaseMaterialsFolder("Assets/" + baseMaterialsFolderName + "/Resources");

                // mi assicuro che venga generata un'eccezione se qualcosa dovesse andare storto durante il parsing
                Assert.raiseExceptions = false;

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
            Assert.AreEqual(instruction.Length, 5);
            current.setColor(Convert.ToSingle(instruction[1]), Convert.ToSingle(instruction[2]), Convert.ToSingle(instruction[3]), Convert.ToSingle(instruction[4]));
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

    private string modelName;
    private string utmFile;
    private string customMaterialsFolder;
    private MaterialDefinition current;

    [SerializeField]
    private string modelFolderPath;
    [SerializeField]
    private bool overwrite = false;
}
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

public class BaseMaterialsGenerator : MonoBehaviour
{
    void Start()
    {
        initFolder();
        generateMaterialsFromTextFile("TabellaCorrispondenzaMateriali.txt");
    }

    private void initFolder()
    {
        if (Directory.Exists(mk_sTesyMaterialsFolder))
        {
            Directory.Delete(mk_sTesyMaterialsFolder);
        }
        Directory.CreateDirectory(mk_sTesyMaterialsFolder);
        Debug.Log("initialized UTesyMaterials folder");
    }

    private void generateMaterialsFromTextFile(string fileName)
    {
        Load(fileName);
    }

    private bool Load(string fileName)
    {
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file was saved as
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);

            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        parseLine(line);
                    }
                }
                while (line != null);
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

        if (line.Substring(0, 2).Equals("//"))
        {
            Debug.Log("commento:\n" + line);
            return;     // commento
        }

        string[] entries = line.Split('|');
        if (entries.Length < mk_iNEntries)
        {
            Debug.Log("not enough entries in this line!");
            return;     // riga errata
        }
        createMaterial(entries);
    }

    private void /*TesyMaterial*/ createMaterial(string[] entries)
    {
        Material material = new Material(Shader.Find("Standard"));
        AssetDatabase.CreateAsset(material, mk_sTesyMaterialsFolder + entries[mk_iNEntries - 1].Trim() + ".mat");
        Debug.Log("created material: " + entries[mk_iNEntries - 1].Trim());
    }

    private const int mk_iNEntries = 7;
    private const string mk_sTesyMaterialsFolder = "Assets/AutoTesyMaterials/";
}

//struct TesyMaterial
//{
//    string FatherMaterialName;
//    string UnityMaterialName;
//}
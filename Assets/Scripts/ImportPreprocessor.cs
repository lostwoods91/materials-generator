using UnityEngine;
using UnityEditor;

//class ImportPreprocessor : AssetPostprocessor
//{
//    void OnPostprocessModel(GameObject g)
//    {
//        //Apply(g.transform);
//        Debug.Log("BAU!");
//    }

//    void Apply(Transform t)
//    {
//        if (t.name.ToLower().Contains("collider"))
//            t.gameObject.AddComponent<MeshCollider>();

//        // Recurse
//        foreach (Transform child in t)
//            Apply(child);
//    }
//}
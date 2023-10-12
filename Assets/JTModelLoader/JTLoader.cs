// @<COPYRIGHT>@
// ==================================================
// Copyright 2017.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@ 

using UnityEngine;
//using JTParser;
using JTForUnity;
//using JTTools;
#if UNITY_EDITOR_64
using UnityEditor;
#endif

/// <summary>
/// Unity Behavior that does the conversion.  Also contains boilerplate code to configure the application environment for any logging and dependency loading.
/// </summary>
public class JTLoader : JTLoaderInternal {


   

    // Use this for initialization
    /// <summary>
    /// API used for Unity Start loop..
    /// </summary>
    void Start () {

        //Convert the designated JSON file to Unity objects.
        Convert();
    }

  
}

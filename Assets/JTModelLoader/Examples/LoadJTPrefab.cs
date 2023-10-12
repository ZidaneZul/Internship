// @<COPYRIGHT>@
// ==================================================
// Copyright 2018.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@ 

using UnityEngine;


/// <summary>
/// Example of how to Load a JT Prefab.
/// </summary>
public class LoadJTPrefab : MonoBehaviour
{

    // Use this for initialization
    public string Prefab = "Prefabs/Radial_Engine";
    public bool CenterOnCamera = true;

    /// <summary>
    /// The prefab go
    /// </summary>
    private GameObject prefabGO;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        //load prefab from resources
        var prefabResource = Resources.Load("Prefabs/Radial_Engine") as GameObject;

        //loaded??
        if (prefabResource != null)
        {
            //Create
            prefabGO = GameObject.Instantiate(prefabResource, transform.position, transform.rotation);

            if (prefabGO != null)
            {
                //created.  Parent to this and focus camera on it.
                prefabGO.transform.SetParent(this.gameObject.transform, false);
                FocusOnModel(prefabGO);
            }
        }


    }

    /// <summary>
    /// Centers the in scene.
    /// </summary>
    /// <param name="gameObject">The game object to focus on</param>
    private void FocusOnModel(GameObject gameObject)
    {
        if (CenterOnCamera)
        {
            var cameraPos = Camera.main.transform.position;
            gameObject.transform.position = cameraPos;
            gameObject.transform.Translate(Camera.main.transform.forward * 1.5f);
            gameObject.transform.forward = Camera.main.transform.forward;
        }
    }
}
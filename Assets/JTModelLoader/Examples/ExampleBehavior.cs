// @<COPYRIGHT>@
// ==================================================
// Copyright 2017.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@ 


using JTForUnity;
using UnityEngine;
using UnityObjects;
using UnityEngine.UI;


/// <summary>
/// Example behavior for how register for the status callback int the JTModelBehavior and do some processing during and after the conversion process.
/// </summary>
public class ExampleBehavior : MonoBehaviour
{
    /// <summary>
    /// Set to true if you want the model to automatically be centered in the scene.
    /// </summary>
    public bool CenterModelOnCamera = true;

    /// <summary>
    /// The scale to apply to the model.  The bigger the value the bigger the model.
    /// </summary>
    public float Scale = 0.50f;


    [HideInInspector]
    private GameObject rootGameObject;

    // Use this for initialization
    /// <summary>
    /// Start api called by the Unity Start loop.
    /// </summary>
    void Start()
    {

        //register for the status callback updates.
        JTLoader jt = GetComponent<JTLoader>();

        if (jt != null && jt.isActiveAndEnabled)
        {
            jt.StatusCallback += StatusCallback;
        }
    }

 


    /// <summary>
    /// Implementation of the status callback delegate.  Use to handle any necessary processing during and after the conversion.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="obj">The object.</param>
    private void StatusCallback(JTModelLoaderConstants.StatusCodes statusCode, object obj)
    {
        bool enabled = true;
        string textValue = "";
        JTLoader jt;

        //handle failure as you see fit.
        switch (statusCode)
        {
            //All done.
            case JTModelLoaderConstants.StatusCodes.Success:

                //example of retrieving object passed at Success.
                NodesCollection collection;

                if (obj is NodesCollection)
                {
                    collection = (NodesCollection)obj;
                }

                textValue = "";
                enabled = false;

                //Destroy the loader.  We are done.
                SafeDestroy(GetComponent<JTLoader>());
             
                break;

            //Structure root has been created.
            case JTModelLoaderConstants.StatusCodes.RootCreated:
                ModelAssemblyComp assemblyRoot = obj as ModelAssemblyComp;
                if (assemblyRoot != null && assemblyRoot.Bounds != JTModelLoaderConstants.ZeroBounds)
                {
                    jt = GetComponent<JTLoader>();

                    if (jt != null && jt.isActiveAndEnabled)
                    {
                        jt.transform.SetParent(this.gameObject.transform, false);

                        rootGameObject = jt.gameObject;

                        if (CenterModelOnCamera)
                        {
                            CenterOnCamera(rootGameObject);
                        }

                        float resizeTo = GetModelScale(assemblyRoot.Bounds);

                        rootGameObject.transform.localScale = new Vector3(resizeTo, resizeTo, resizeTo);

                        var nb = FindObjectOfType<NavigationBehavior>();
                        if (nb != null)
                        {
                            nb.LoadingRoot = rootGameObject;
                        }



                    }
                }
                break;
            //Error case.
            case JTModelLoaderConstants.StatusCodes.JSONFileNotFound:
                textValue = "JSON file was not found at specified location!";
                break;
            case JTModelLoaderConstants.StatusCodes.ErrorLoadingJSON:
                textValue = "JSON file either not found or contained errors";
                break;
        }

        // sample scenes have a canvas in them to handle text messages.
        Canvas can = FindObjectOfType<Canvas>();
        if (can)
        {
            can.enabled = enabled;
        }

        Text text = FindObjectOfType<Text>();
        if (text)
        {
            text.text = textValue;
        }

    }


    /// <summary>
    /// Centers the JT game object in the scene.
    /// </summary>
    /// <param name="gameObject">The game objec to center.</param>
    private void CenterOnCamera(GameObject gameObject)
    {
        var cameraPos = Camera.main.transform.position;
        gameObject.transform.position = cameraPos;
        gameObject.transform.Translate(Camera.main.transform.forward * 1.5f);
        gameObject.transform.forward = Camera.main.transform.forward;
    }


    /// <summary>
    /// Gets the JT model scale so it can be sized properly within the scene.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <returns>scale</returns>
    private float GetModelScale(Bounds bounds)
    {
        float maxSize = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
        if (maxSize > 0)
        {
            float resizeTo = Scale / maxSize;
            return resizeTo;
        }

        return 1;
    }

    /// <summary>
    /// Destroys the specified object, in Editor or a compiled app.
    /// </summary>
    /// <param name="target">The target.</param>
    public static void SafeDestroy(Object target)
    {
        if (target != null)
        {
            if (Application.isEditor)
                Object.DestroyImmediate(target);
            else
                Object.Destroy(target);
        }

    }
}

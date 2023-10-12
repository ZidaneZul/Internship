// @<COPYRIGHT>@
// ==================================================
// Copyright 2018.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Example of a behavior that allows the editor to create a prefab from a running JT model Asset.
/// Resulting Prefab will have no references to any other JT loader module but UnityObjects.  This allows the prefab to be used cross-platform
/// and pass the windows certification test.
/// </summary>
public class CreateJTPrefabAsset : MonoBehaviour
{
    /// <summary>
    /// The prefab output file name and path (must be in resources directory)
    /// </summary>
    private string _prefabFileNameAndPath;

    /// <summary>
    /// The game object to clone (based on selection)
    /// </summary>
    [HideInInspector]
    private GameObject _gameObjectToClone;

    /// <summary>
    /// counters for the progress bar.
    /// </summary>
    private int _busyCount = 0, _totalCount = 0;

    /// <summary>
    /// flags to indicate all prefab processing has been finished.
    /// </summary>
    private bool _meshesFinished = false, _linesFinished = false;

    /// <summary>
    /// The mesh processing coroutine
    /// </summary>
    private Coroutine meshRoutine=null;

    /// <summary>
    /// The line processing coroutine
    /// </summary>
    private Coroutine lineRoutine=null;

    /// <summary>
    /// The empty prefab that will be populated.
    /// </summary>
    UnityEngine.Object _emptyPrefab;

    /// <summary>
    /// Menu item method exposed to editor, under GameObject, called Create JT Prefab Asset
    /// </summary>
    [MenuItem("GameObject/Create JT Prefab Asset")]
    static void Create()
    {
        //Find the active gameObject and execute prefab creation.
        GameObject activeGameObject = Selection.activeGameObject;

        if (activeGameObject != null)
        {
            GameObject createPrefabObject = new GameObject("CreatePrefab");
            CreateJTPrefabAsset cja = createPrefabObject.AddComponent<CreateJTPrefabAsset>();
            cja.CreatePrefab(activeGameObject);
        }
        else
        {
            EditorUtility.DisplayDialog("Error!", "No game object selected in hiearchy!", "Ok");
        }
    }

    /// <summary>
    /// Safely destroys given object, either in editor or running application.
    /// </summary>
    /// <param name="target">The target.</param>
    public static void SafeDestroy(UnityEngine.Object target)
    {
        if (target != null)
        {
            if (Application.isEditor)
                UnityEngine.Object.DestroyImmediate(target);
            else
                UnityEngine.Object.Destroy(target);
        }

    }

    /// <summary>
    /// Creates the prefab.
    /// </summary>
    public void CreatePrefab(GameObject gameObjectToClone)
    {
        _gameObjectToClone = gameObjectToClone;
        StartCoroutine(ProcessToPrefab());
    }

    /// <summary>
    /// Creates the prefab.  Implemented as a coroutine to allow the example behavior, if attached, to be removed from the prefab.  The resulting prefab must have no dependencies
    /// on JTForUnity, JTParser, etc, only UnityObjects in order to avoid cross platform issues and windows store certification issues.
    /// </summary>
    /// <returns>enumerator</returns>
    private IEnumerator ProcessToPrefab()
    {


        //initialize counters flag.
        Init();

        if (_gameObjectToClone != null)
        {
            //get new prefab filename and location.  (must be under resources)
            _prefabFileNameAndPath = EditorUtility.SaveFilePanelInProject(" Output File must be under resources", _gameObjectToClone.name +".prefab", "prefab", "Cloning game object: " + _gameObjectToClone.name);

            if (!string.IsNullOrEmpty(_prefabFileNameAndPath))
            {
                //create empty prefab.
                _emptyPrefab = PrefabUtility.CreateEmptyPrefab(_prefabFileNameAndPath);

                //add meshes, materials, etc to prefab.
                AddResourcesToPrefab(_emptyPrefab, _gameObjectToClone);
            }

        }
        else
        {
            EditorUtility.DisplayDialog("Error!", "No game object selected in hiearchy!", "Ok");
        }

        yield return null;
    }

    /// <summary>
    /// Initializes counters and flags.
    /// </summary>
    private void Init()
    {
        _meshesFinished = _linesFinished = false;
        meshRoutine = lineRoutine = null;
        _busyCount = _totalCount = 0;
    }


    /// <summary>
    /// Populate a empty prefab starting at the designated game object.
    /// </summary>
    /// <param name="emptyPrefab">The empty prefab to populate</param>
    /// <param name="gameObject">The game object to use to populate prefab.</param>
    private void AddResourcesToPrefab(UnityEngine.Object emptyPrefab, GameObject gameObject)
    {

        if (emptyPrefab != null && gameObject != null)
        {
            EditorUtility.DisplayProgressBar("Progress", "Creating the prefab......", 0f);

            //get the mesh and line renders to add to prefab.
            var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            LineRenderer[] lineRenderers = gameObject.GetComponentsInChildren<LineRenderer>(true);

            _totalCount = meshRenderers.Length + lineRenderers.Length;

            //process mesh renders.
            if (meshRenderers != null && meshRenderers.Length > 0)
            {
                meshRoutine = StartCoroutine(ProcessMeshes(emptyPrefab, meshRenderers));
            }
            else
            {
                _meshesFinished = true;
            }

            //process line renders.
            if (lineRenderers != null)
            {
                lineRoutine = StartCoroutine(ProcessLines(emptyPrefab, lineRenderers));
            }
            else
            {
                _linesFinished = true;
            }
        }

    }


    /// <summary>
    /// Handles progress notification and cancel requests.
    /// </summary>
    private void HandleProgress()
    {

        if (_totalCount > 0 )
        {
            float percentComplete = (float)_busyCount / (float)_totalCount;

            if (EditorUtility.DisplayCancelableProgressBar("Progress", "Creating the prefab......" + _busyCount + " of " + _totalCount, percentComplete))
            {
                //cancel was clicked, stop the coroutines.
                if (meshRoutine != null)
                {
                    StopCoroutine(meshRoutine);
                }

                if (lineRoutine != null)
                {
                    StopCoroutine(lineRoutine);
                }

                //remove partial file.
                FileUtil.DeleteFileOrDirectory(_prefabFileNameAndPath);
                FileUtil.DeleteFileOrDirectory(_prefabFileNameAndPath + ".meta");

                //blank everything to keep this code from executing again on this prefab creation.
                Init();

                EditorUtility.ClearProgressBar();

                //delete after done
                Destroy(this.gameObject);
            }
        }

    }

    /// <summary>
    /// Processes the lines renders
    /// </summary>
    /// <param name="emptyPrefab">The empty prefab to be populated</param>
    /// <param name="lineRenderers">The line renderers to be populated</param>
    /// <returns>ienumerator for coroutine.</returns>
    private IEnumerator ProcessLines(UnityEngine.Object emptyPrefab, LineRenderer[] lineRenderers)
    {
        //No point in starting this until meshes are done (if there were any).
        yield return new WaitUntil(() => _meshesFinished);

        //any line renderers?
        if (lineRenderers != null)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int materialIndex = 1;

            List<UnityEngine.Object> processedTextAndMat = new List<UnityEngine.Object>();

            foreach (var r in lineRenderers)
            {

                //yield every once in a while to give the progress bar/cancel some process time.
                if (watch.ElapsedMilliseconds > 1000)
                {
                    yield return null;
                    watch.Reset();
                    watch.Start();
                }

                _busyCount++;
            

                r.name = "line" + materialIndex;

                //Process this renderers materials.
                foreach (Material material in r.sharedMaterials)
                {
                    if (!AssetDatabase.Contains(material) && !processedTextAndMat.Contains(material))
                    {
                        //shader name for textures that get added.
                        Texture texture = material.GetTexture("_MainTex");

                        if (texture != null && !processedTextAndMat.Contains(texture))
                        {                            
                            texture.name = "texture" + materialIndex;

                            AssetDatabase.AddObjectToAsset(texture, emptyPrefab);
                            processedTextAndMat.Add(texture);

                        }
                       
                        material.name = "material" + materialIndex;

                        AssetDatabase.AddObjectToAsset(material, emptyPrefab);
                        processedTextAndMat.Add(material);


                    }
                }
                materialIndex++;

            }

            _linesFinished = true;

        }
    }



    /// <summary>
    /// Processes the meshes.
    /// </summary>
    /// <param name="emptyPrefab">The empty prefab.</param>
    /// <param name="meshRenderers">The mesh renderers.</param>
    /// <returns></returns>
    private IEnumerator ProcessMeshes(UnityEngine.Object emptyPrefab, MeshRenderer[] meshRenderers)
    {
        if (meshRenderers != null)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            int processedMeshCount = 0;
            int meshIndex = 0;

            IDictionary<Mesh, int> addedFilters = new Dictionary<Mesh, int>();
            List<UnityEngine.Object> processedTextAndMat = new List<UnityEngine.Object>();

            foreach (var r in meshRenderers)
            {
                //yield every once in a while to give the progress bar/cancel some process time.
                if (watch.ElapsedMilliseconds > 1000)
                {
                    yield return null;
                    watch.Reset();
                    watch.Start();
                }

                _busyCount++;

          
                GameObject go = r.gameObject;
                MeshFilter mf = go.GetComponent<MeshFilter>();

                if (mf != null)
                {
                    //Haven't seen this mesh before.
                    if (!addedFilters.ContainsKey(mf.sharedMesh))
                    {
                        processedMeshCount++;
                      
                        mf.sharedMesh.name = "mesh" + processedMeshCount;
                        meshIndex = processedMeshCount;

                        addedFilters.Add(mf.sharedMesh, processedMeshCount);
                        if (!AssetDatabase.Contains(mf.sharedMesh))
                        {
                            AssetDatabase.AddObjectToAsset(mf.sharedMesh, emptyPrefab);
                        }
                    }
                    //Seen it, retrieve its index.
                    else
                    {
                        addedFilters.TryGetValue(mf.sharedMesh, out meshIndex);
                    }

                    //Process this meshes materials.
                    foreach (Material material in r.sharedMaterials)
                    {
                        if (!AssetDatabase.Contains(material) && !processedTextAndMat.Contains(material))
                        {
                            //Steve's name for textures that get added.
                            Texture texture = material.GetTexture("_MainTex");

                            if (texture != null && !processedTextAndMat.Contains(texture))
                            {

                                texture.name = "texture" + meshIndex;

                                AssetDatabase.AddObjectToAsset(texture, emptyPrefab);
                                processedTextAndMat.Add(texture);
                            }

                            material.name = "material" + meshIndex;

                            AssetDatabase.AddObjectToAsset(material, emptyPrefab);
                            processedTextAndMat.Add(material);

                        }
                    }

                }

            }

            addedFilters.Clear();
            addedFilters = null;

            processedTextAndMat.Clear();
            processedTextAndMat = null;

            _meshesFinished = true;
        }
    }

    /// <summary>
    /// Update callback (unity)
    /// </summary>
    private void Update()
    {
        //finished processing?
        if (_meshesFinished && _linesFinished)
        {
            //blank everything to keep this code from executing again on this prefab creation.
            Init();

            //populate prefab.
            GameObject prefabRootGameObject = PrefabUtility.ReplacePrefab(_gameObjectToClone, _emptyPrefab);

            //Destroy the JTLoader script on the prefab
            JTLoader loader = prefabRootGameObject.GetComponent<JTLoader>();
            if (loader != null)
            {
                DestroyImmediate(loader, true);
            }

            //if there are any other scripts on the prefab the user doesn't need, Then they need to delete it, either by adding code or deleting it manually
           

            //cleanup.
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            //delete after done
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Editor gui callback
    /// </summary>
    void OnGUI()
    {
        HandleProgress();
    }

    /// <summary>
    /// ondestroy callback.
    /// </summary>
    private void OnDestroy()
    {
        EditorUtility.ClearProgressBar();
    }

}
#endif

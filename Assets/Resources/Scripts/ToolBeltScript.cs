using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBeltScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera camera;
    public Vector3 offset = new Vector3 (0f, 0.7f, 0f);
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.transform.position - offset;
    }
}

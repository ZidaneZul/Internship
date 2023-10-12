// @<COPYRIGHT>@
// ==================================================
// Copyright 2017.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@ 


using UnityEngine;

/// <summary>
/// Example behavior that shows how to implement Pan (right mouse button), Zoom (scroll wheel), and Rotate (left mouse button)
/// </summary>
public class NavigationBehavior : MonoBehaviour
{


    /// <summary>
    /// The horizontal rotation speed
    /// </summary>
    public float horizontalRotationSpeed = 2.0F;

    /// <summary>
    /// The vertical rotation speed
    /// </summary>
    public float verticalRotationSpeed = 2.0F;

    /// <summary>
    /// The mouse zoom speed
    /// </summary>
    public float mouseZoomSpeed = 50f;

    /// <summary>
    /// The mouse zoom speed button
    /// </summary>
    public float mouseZoomSpeedButton = 10f;

    /// <summary>
    /// The values for tracking rotation.
    /// </summary>
    private float _h = 0.0f, _v = 0.0f;

    /// <summary>
    /// The previous mouse position (tracking while mouse down)
    /// </summary>
    private Vector3 _previousMousePos = new Vector3(9999, 9999);

    /// <summary>
    /// The structure and pmi (all visible game objects) root.
    /// </summary>
    [HideInInspector]
    public GameObject LoadingRoot;


    /// <summary>
    /// Handles the interaction with mouse.
    /// </summary>
    protected void HandleInteraction()
    {
        if (LoadingRoot)
        {
            //if mouse up reset previous mouse position.
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                _previousMousePos = new Vector3(9999, 9999);
            }
            else
            {
                if (Input.GetMouseButton(0))//left  Rotate
                {

                    //Feels like the first time.
                    if (_previousMousePos.x == 9999)
                    {
                        _previousMousePos = Input.mousePosition;
                    }
                    //Calculate the deletas.
                    _h += horizontalRotationSpeed * (_previousMousePos.x - Input.mousePosition.x);
                    _v += horizontalRotationSpeed * (_previousMousePos.y - Input.mousePosition.y);

                    //update the previous position.
                    _previousMousePos = Input.mousePosition;

                    //Rotate that puppy.
                    if (_h != 0 || _v != 0)
                    {
                        LoadingRoot.transform.eulerAngles = new Vector3(_v, _h, 0.0f);
                    }
                }
                else
                {
                    if (Input.GetMouseButton(1))//right  Pan.
                    {
                        //Feels like the first time.
                        if (_previousMousePos.x == 9999)
                        {
                            _previousMousePos = Input.mousePosition;
                        }
                        else
                        {

                            //Where it was.
                            Vector3 mousePosition = new Vector3(_previousMousePos.x, _previousMousePos.y, LoadingRoot.transform.position.z);
                            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                            //Where it will be.
                            Vector3 mousePosition1 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, LoadingRoot.transform.position.z);
                            mousePosition1 = Camera.main.ScreenToWorldPoint(mousePosition1);

                            //duh delta.
                            Vector3 delta = mousePosition1 - mousePosition;

                            //move it.
                            LoadingRoot.transform.position += delta;
                        }

                        _previousMousePos = Input.mousePosition;
                    }
                    else
                    {
                        handleZoomWheel();  //Scroll.
                    }
                }
            }
        }


    }


    /// <summary>
    /// Handles the zoom wheel.  Just change the field of view for zooming in and out.
    /// </summary>
    private void handleZoomWheel()
    {

        float movement = Input.GetAxisRaw("Mouse ScrollWheel");

        if (movement != 0)
        {
            float curZoomPos;
            curZoomPos = Camera.main.fieldOfView;
            curZoomPos -= movement * mouseZoomSpeed;

            curZoomPos = Mathf.Clamp(curZoomPos, 1.0f, 150f);
            Camera.main.fieldOfView = curZoomPos;
        }
    }



    /// <summary>
    /// API for Unity LateUpdate loop.
    /// </summary>
    void LateUpdate()
    {

        HandleInteraction();


    }


}

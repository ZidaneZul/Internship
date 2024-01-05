using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorImageScript : MonoBehaviour
{
    public Camera camera;

    public Image image;

    float timer;

    bool flipBool;

    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        image = GetComponent<Image>();

        image.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = camera.transform.forward;

        if (Input.GetButton("Jump"))
        {
            isEnabled = !isEnabled;
        }

        if (isEnabled)
        {
            image.color = Color.white;
            BlinkingTimer(1f);
            Blinking(flipBool);
        }
        else
        {
            image.color = Color.clear;
        }
    }

    void Blinking(bool boo)
    {
        if (boo)
            image.color = Color.clear;
        else
            image.color = Color.white;
    }

    void BlinkingTimer(float time)
    {
        timer += Time.deltaTime;

        if(timer > time)
        {
            flipBool = !flipBool;
            timer = 0f;
        }
    }
}

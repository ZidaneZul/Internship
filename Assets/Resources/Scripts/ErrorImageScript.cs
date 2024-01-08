using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorImageScript : MonoBehaviour
{
    public Camera camera;
    public AudioSource audioSource;
    public Image image;

    float timer;

    bool flipBool;

    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        image.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = camera.transform.forward;

        if (Input.GetKeyDown(KeyCode.W))
            isEnabled = !isEnabled;

        if (isEnabled)
        {
            image.color = Color.white;
            BlinkingTimer(1f);
            Blinking(flipBool);
            Debug.Log("Playing sound");
            audioSource.Play();
        }
        else
        {
            image.color = Color.clear;
            Debug.Log("not playing sound");
            audioSource.Stop();
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

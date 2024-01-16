using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayerScript : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = player.transform.forward;
    }
}

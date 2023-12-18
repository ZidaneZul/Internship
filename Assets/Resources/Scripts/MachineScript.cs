using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    Animator anim;


    public GameObject claw_GO;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayPickAnim()
    {
        anim.SetTrigger("Pick");
    }

    void PlayPlaceAnim()
    {
        anim.SetTrigger("Place");
    }

    void PickItemUp(GameObject item)
    {
        item.transform.parent = claw_GO.transform;
    }

}

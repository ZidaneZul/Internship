using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimationScript : MonoBehaviour
{
    Animator anim;

    public GameObject Claw_GO;

    public GameObject carrier, pointInMachine;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
      
    }

    /// <summary>
    /// Would rather use triggers but its running mulitple times so using bool to make it
    /// run once
    /// </summary>
    public void PlayPickAnim()
    {
        anim.SetBool("Pick", true);
    }

    public void PlayPlaceAnim()
    {
        anim.SetBool("Place", true);
    }

    public void ResetBools()
    {
        anim.SetBool("Pick", false);
        anim.SetBool("Place", false);
    }


    public void PickItemUp(GameObject item)
    {
        item.transform.parent = Claw_GO.transform;
    }
    public void PlaceItemOnCarrier(GameObject gameObject)
    {
        gameObject.transform.parent = carrier.transform;
    }
    public void PlaceItemInside(GameObject GO)
    {
        GO.transform.parent = pointInMachine.transform;
    }

    

}

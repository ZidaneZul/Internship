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

    public void PlayPickAnim()
    {
        anim.SetTrigger("Pick");
    }

    public void PlayPlaceAnim()
    {
        anim.SetTrigger("Place");
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

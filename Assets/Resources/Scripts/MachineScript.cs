using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    public GameObject pickRobot, placeRobot;

    Animator pickRobot_anim, placeRobot_anim;

    RobotAnimationScript pickRobot_animScript, placeRobot_animScript;

    float time = 0;


    // Start is called before the first frame update
    void Start()
    {
        pickRobot_anim = pickRobot.GetComponent<Animator>();
        placeRobot_anim = placeRobot.GetComponent<Animator>();
        
        pickRobot_animScript = pickRobot.GetComponent<RobotAnimationScript>();
        placeRobot_animScript = placeRobot.GetComponent<RobotAnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayPickAnimation()
    {
        pickRobot_animScript.PlayPickAnim();
    }

    public void PlayPlaceAnimation()
    {
        placeRobot_animScript.PlayPlaceAnim();
    }
    public bool Timer(float seconds)
    {
        Debug.Log("TESTINTES timer");
        time += Time.deltaTime;
        while (time < seconds)
        {
            Debug.Log("returning false");

            return false;
        }
        Debug.Log("returning true");
        time = 0;
        return true;
    }
}

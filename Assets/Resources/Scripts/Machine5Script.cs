using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine5Script : MonoBehaviour
{
    public int materialRemainingInside;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsThereMaterialsLeft()
    {
        if (materialRemainingInside > 0)
        {
            return true;
        }
        return false;
    }

    public void IncreaaseMaterialCount()
    {
        materialRemainingInside = 6;    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolArea : MonoBehaviour
{
    public bool onPatrolArea = false;


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            onPatrolArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            onPatrolArea = false;
        }
    }
}

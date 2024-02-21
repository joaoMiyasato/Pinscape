using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestUi : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public Rigidbody playerRb;
    
    void Start()
    {
        
    }

    void Update()
    {
        //tmp.text = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z).magnitude.ToString();
    }
}

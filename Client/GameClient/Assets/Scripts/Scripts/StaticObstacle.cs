using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObstacle : MonoBehaviour
{
    public int id = 0;
    public MeshRenderer model;

    private void FixedUpdate()
    {
        
    }

    public void Initialise(int identity)
    {
        id = identity;
    }
}


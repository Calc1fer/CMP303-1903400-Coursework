using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;

public class SpinObstacle : MonoBehaviour
{
    public int id = 0;
    public MeshRenderer model;
    private Quaternion rotation = Quaternion.identity;

    private void FixedUpdate()
    {
        //transform.Rotate(Vector3.up, rotation.w * Time.deltaTime, Space.World);
        //Debug.Log($"{rotation.w}");
    }
    public void Initialise(int obstacle_id)
    {
        id = obstacle_id;
    }

    public void Rotate(Quaternion rot)
    {
        rotation = rot; 
    }
}

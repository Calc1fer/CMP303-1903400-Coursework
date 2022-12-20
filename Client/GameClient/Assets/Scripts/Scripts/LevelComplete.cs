using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public int id;
    public MeshRenderer model;
  public void Initialise(int identity)
    {
        id = identity;
        model.enabled = true;
    }

    public void Spawned()
    {
        model.enabled = true;
    }

    public void WinCondition()
    {

    }
}

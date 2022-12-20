using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int spawner_id;
    public bool picked_up;
    public MeshRenderer collectible_model;

    private Vector3 base_pos;

    public void Initialise(int spawner_identity, bool picked)
    {
        spawner_id = spawner_identity;
        picked_up = picked;
        collectible_model.enabled = picked;

        base_pos = transform.position;
    }
    
    public void ItemSpawned()
    {
        picked_up = true;
        collectible_model.enabled = true;
    }

    public void ItemPickedUp()
    {
        picked_up = false;
        collectible_model.enabled = false;
    }
}

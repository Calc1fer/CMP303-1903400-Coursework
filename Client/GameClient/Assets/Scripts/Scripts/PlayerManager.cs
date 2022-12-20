using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float max_health;
    public float max_collectibles = 3;
    public int collectible_count = 0;
    public MeshRenderer model;

    public void Initialise(int identity, string name)
    {
        id = identity;
        username = name;
        health = max_health;
    }

    public void Sethealth(float hp)
    {
        health = hp;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        Sethealth(max_health);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    public static NetManager instance;

    public GameObject player_prefab;
    public GameObject projectile_prefab;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying objects...");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player InstantiatePlayer()
    {
        return Instantiate(player_prefab, new Vector3(0f, 1.08f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(Transform shoot_origin)
    {
        return Instantiate(projectile_prefab, shoot_origin.position + shoot_origin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
}

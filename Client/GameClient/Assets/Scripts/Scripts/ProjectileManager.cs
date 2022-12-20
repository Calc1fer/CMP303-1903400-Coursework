using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int id;
    public GameObject projectile_prefab;

    public void Initialise(int identity)
    {
        id = identity;
    }

    public void DestroyProjectiles(Vector3 pos)
    {
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }
}

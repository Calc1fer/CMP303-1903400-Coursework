using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int projectile_id = 1;

    public int id;
    public Rigidbody body;
    public int by_player;
    public Vector3 init_force;
    public float damage = 50f;
    public float distance = 1;

    private void Start()
    {
        id = projectile_id;
        projectile_id++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(by_player, this);

        body.AddForce(init_force);
        StartCoroutine(DestroyAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage();
    }

    public void Initialise(Vector3 init_move_dir, float init_force_strength, int thrown)
    {
        init_force = init_move_dir * init_force_strength;
        by_player = thrown;
    }

    private void Damage()
    {
        ServerSend.ProjectileDestroyed(this);

        Collider[] colliders = Physics.OverlapSphere(transform.position, distance);

        foreach(Collider collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                collider.GetComponent<Player>().PlayerDamage(damage);
            }
        }
        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(3f);
        Damage();
    }
}

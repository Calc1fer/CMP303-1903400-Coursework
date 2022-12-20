using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static Dictionary<int, Collectible> spawners = new Dictionary<int, Collectible>();
    private static int next_spawner_id = 1;

    public int spawner_id;
    public bool picked_up = false;

    private void Start()
    {
        picked_up = false;
        spawner_id = next_spawner_id;
        next_spawner_id++;
        spawners.Add(spawner_id, this);

        StartCoroutine(SpawnItem());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(picked_up && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player.AttemptPickupItem())
            {
                ItemPickedUp(player.id);
            }
        }
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(5f);

        picked_up = true;
        ServerSend.ItemSpawned(spawner_id);
    }

    private void ItemPickedUp(int by_player)
    {
        picked_up = false;
        ServerSend.ItemPickedUp(spawner_id, by_player);

        StartCoroutine(SpawnItem());
    }
}

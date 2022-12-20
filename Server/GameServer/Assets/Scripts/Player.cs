using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string name;

    public Quaternion rotation = new Quaternion(0, 0, 0, 0);
    public Transform shoot_origin;
    public float move_speed = 5f;
    public float rotation_speed = 720f;
    public float rot = 90f;
    public float health;
    public float max_health = 100f;
    public float shoot_force = 700f;
    public int collectibles = 0;
    public int max_collectibles = 3;
    private bool[] inputs;
    bool is_shooting;
    private float timer = 0;


    public CharacterController player_controller;

    Projectile projectile;

    private void Start()
    {
        move_speed *= Time.fixedDeltaTime;
    }
    public void Initialise(int identity, string username)
    {
        id = identity;
        name = username;
        health = max_health;

        inputs = new bool[6];
       // projectile = new Projectile(id);
        is_shooting = false;
    }

    public void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(health <= 0f)
        {
            return;
        }

        Vector2 input_dir = Vector2.zero;

        if (inputs[0])
        {
            input_dir.y += 1;
        }
        if (inputs[1])
        {
            input_dir.y -= 1;
        }
        if (inputs[2])
        {
            input_dir.x -= 1;
        }
        if (inputs[3])
        {
            input_dir.x += 1;
        }
        if (inputs[4])
        {
            //Q
            Debug.Log("rotate left");
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + (rot * Time.fixedDeltaTime), transform.rotation.z);
        }
        if (inputs[5])
        {
            //E
            Debug.Log("rotate right");
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y - (rot * Time.fixedDeltaTime), transform.rotation.z);
        }

        Move(input_dir);
    }

    private void Move(Vector2 input_dir)
    {

        Vector3 move_dir = transform.right * input_dir.x + transform.forward * input_dir.y;
        move_dir *= move_speed;

        if (transform.position.y > 1.08)
        {
            transform.position = new Vector3(transform.position.x, 1.08f, transform.position.z);
        }

        if (transform.position != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotation_speed * Time.deltaTime);
        }

        player_controller.Move(move_dir);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void Interpolate(List<Vector4>pos_vals)
    {
        //Using Quadratic
        //s = ut + 1/2at^2
        Vector2 pos_diff;
        pos_diff = new Vector2(Mathf.Abs(pos_vals[0].x - pos_vals[1].x), Mathf.Abs(pos_vals[0].z - pos_vals[1].z));
        float xz_diff = Mathf.Sqrt(Mathf.Pow(pos_diff.x, 2f) + Mathf.Pow(pos_diff.y, 2f));
        float time_diff = Mathf.Abs(pos_vals[1].w - pos_vals[0].w);
        float speed = Mathf.Abs(xz_diff / time_diff);

        Vector2 pos_diff2;
        pos_diff2 = new Vector2(Mathf.Abs(pos_vals[1].x - pos_vals[2].x), Mathf.Abs(pos_vals[1].z - pos_vals[2].z));
        float xz_diff2 = Mathf.Sqrt(Mathf.Pow(pos_diff2.x, 2f) + Mathf.Pow(pos_diff2.y, 2f));
        float time_diff2 = Mathf.Abs(pos_vals[2].w - pos_vals[1].w);
        float speed2 = Mathf.Abs(xz_diff2 / time_diff2);

        //Acceleration = diff between speed at last two positions / time between said positions
        float speed_diff = Mathf.Abs(speed - speed2);
        float time_between_pos = Mathf.Abs(time_diff - time_diff2);
        float acc = 0;

        //time since last position
        float time_since = Mathf.Abs(timer - pos_vals[2].w);

        //displacement = speed(most recent position) * time(since last position) + 0.5 * acc * time^2
        float displacement = (speed2 * time_since) + (0.5f * acc * Mathf.Pow(time_since, 2f));

        //Set the transform of the player position to the final values here
        transform.position = new Vector3(transform.position.x + displacement, transform.position.y, transform.position.z + displacement);

    }

    public void SetInput(bool[] _inputs, Quaternion rot)
    {
        inputs = _inputs;
        transform.rotation = rot;
    }

    public void Shoot(Vector3 view_dir)
    {
        if(health <= 0)
        {
            return;
        }

        NetManager.instance.InstantiateProjectile(shoot_origin).Initialise(view_dir, shoot_force, id);

        //if (Physics.Raycast(shoot_origin.position, view_dir, out RaycastHit hit, 25f))
        //{
        //    if (hit.collider.CompareTag("Player"))
        //    {
        //        hit.collider.GetComponent<Player>().PlayerDamage(50f);
        //    }
        //}
    }

    public void PlayerDamage(float damage)
    {
        if (health <= 0)
        {
            return;
        }

        health -= damage;

        if(health <= 0)
        {
            health = 0;
            player_controller.enabled = false;
            transform.position = new Vector3(0f, 1.08f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerPosition(this);
        
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);

        health = max_health;
        player_controller.enabled = true;
        ServerSend.PlayerRespawn(this);
    }

    public bool AttemptPickupItem()
    {
        if(collectibles >= max_collectibles)
        {
            return false;
        }

        collectibles ++;
        return true;
    }

    public int GetScore()
    {
        return collectibles;
    }
}

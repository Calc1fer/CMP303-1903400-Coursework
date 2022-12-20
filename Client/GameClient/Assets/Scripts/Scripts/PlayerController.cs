using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Transform player_cam;
    public float move_speed = 5f;
    public float rot_speed = 150f;
    public float rot = 1f;
    private float timer = 0;
    List<Vector4> pos_vals = new List<Vector4>();
    private int tot_pos_vals = 3;

    private void Update()
    {
        timer += Time.fixedDeltaTime;

        //fill the list of vectors with positions of the player and times when the player were at said positions
        if (pos_vals.Count < tot_pos_vals)
        {
            pos_vals.Add(new Vector4(transform.position.x, transform.position.y, transform.position.z, timer));
        }

        //Once the vectors have been filled, shift the values along one index to the left and refill the last vector
        if(pos_vals.Count >= tot_pos_vals)
        {
            pos_vals[0] = pos_vals[1];
            pos_vals[1] = pos_vals[2];
            
            pos_vals[2] = new Vector4(transform.position.x, transform.position.y, transform.position.z, timer);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            ClientSend.PlayerShoot(transform.forward);
        }
    }

    private void FixedUpdate()
    {
         SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Q),
            Input.GetKey(KeyCode.E),
        };

        ClientSend.PlayerMovement(inputs);

        if(pos_vals.Count < 3)
        {
            return;
        }

        ClientSend.InterpolateMovement(pos_vals);
    }
}

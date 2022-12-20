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
            
            //this will always be the newest time and positions of the player objects
            pos_vals[2] = new Vector4(transform.position.x, transform.position.y, transform.position.z, timer);
        }

        //Shoot functionality for the players
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
        //Preferably not use this, the traditional getAxis input in unity does the trick, but I had difficulty 
        //with transformations so had to keep using this
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Q), //For rotations (not working as desired)
            Input.GetKey(KeyCode.E), //"" "" ""
        };

        ClientSend.PlayerMovement(inputs);

        //Return out of function if the list of vectors does not contain 3 elements
        if(pos_vals.Count < 3)
        {
            return;
        }

        //Otherwise let's send the list to the server after writing to the packet
        ClientSend.InterpolateMovement(pos_vals);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clamp_angle = -85;

    private float vertical_rot;
    private float horizontal_rot;

    private void Start()
    {
        vertical_rot = transform.localEulerAngles.x;
        horizontal_rot = transform.localEulerAngles.y;
    }

    private void Update()
    {
        Look();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float mouse_vertical = Input.GetAxis("Mouse Y");
        float mouse_horizontal = Input.GetAxis("Mouse X");

        vertical_rot += 0;
        horizontal_rot += mouse_horizontal * sensitivity * Time.deltaTime;

        vertical_rot = Mathf.Clamp(vertical_rot, -clamp_angle, clamp_angle);

        transform.localRotation = Quaternion.Euler(vertical_rot, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontal_rot, 0f);
    }
}

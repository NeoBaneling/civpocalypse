using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speed = 0.5f;
    private float minY = 4f;
    private float maxY = 16f;

    // Rotation vars
    private float smooth = 0.5f;
    private float minXRot = 45f;
    private float maxXRot = 75f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateKeyboardInput();

        float mouseScrollDelta = Input.mouseScrollDelta.y * 0.1f;
        if (transform.position.y + mouseScrollDelta > minY && transform.position.y + mouseScrollDelta < maxY)
        {
            transform.position = transform.position + new Vector3(0, mouseScrollDelta, 0);
        }
    }

    private void UpdateKeyboardInput()
    {
        // Moves the camera around according to user input
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(0, 0, speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(-1 * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(0, 0, -1 * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(speed, 0, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speedLat = 5.0f;
    private float speedY = 0.5f;
    private float minY = 4f;
    private float maxY = 36f;

    // Rotation vars
    private float smooth = 0.5f;
    private float minXRot = 45f;
    private float maxXRot = 75f;

    private bool atTarget = true;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
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
        // This is triggered whenever we have a unit/ city/ etc. that the camera
        // needs to move to.
        if (!atTarget)
        {
            float step = speedLat * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            if (transform.position == targetPos)
            {
                atTarget = true;
            }
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("UnitGeneratedEvent", MoveCameraToUnit);
    }

    void OnDisable()
    {
        EventManager.StopListening("UnitGeneratedEvent", MoveCameraToUnit);
    }

    private void UpdateKeyboardInput()
    {
        // Moves the camera around according to user input
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(0, 0, speedY);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(-1 * speedY, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(0, 0, -1 * speedY);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(speedY, 0, 0);
        }
    }

    private void MoveCameraToUnit()
    {
        GameObject unit = UnitManager.Instance.selectedUnit;
        int[] coords = unit.GetComponent<Unit>().GetCoords();
        atTarget = false;
        targetPos = new Vector3(coords[0]*3, minY, coords[1]*3-1.5f);
    }
}

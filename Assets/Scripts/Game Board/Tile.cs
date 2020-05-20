using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string type;
    public GameObject highlight;

    private bool mouseOver = false;
    private GameObject generatedHighlight;

    private int x;
    private int z;

    private float _height;
    public float height { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        // Removes the "(Clone)" at the end
        type = name.Substring(0, name.Length - 7);
        height = transform.lossyScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseOver();
        if (mouseOver)
        {
            CheckMouseClick();
        }
    }

    private void CheckMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit))
        {
            if (!mouseOver && hit.transform.gameObject == this.gameObject)
            {
                generatedHighlight = Instantiate(highlight, transform.position + new Vector3(0f, transform.lossyScale.y/2 + 0.1f, 0f), Quaternion.identity, null);
                mouseOver = !mouseOver;
                tag = "TileSelected";
            }
            else if (mouseOver && !(hit.transform.gameObject == generatedHighlight || hit.transform.gameObject == this.gameObject))
            {
                mouseOver = !mouseOver;
                tag = "TileDeselected";
                Destroy(generatedHighlight);
            }
        }
    }

    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventManager.TriggerEvent("TileClickEvent");
        }
    }

    public void SetCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public int[] GetCoords()
    {
        int[] arr = new int[2];
        arr[0] = x;
        arr[1] = z;
        return arr;
    }
}

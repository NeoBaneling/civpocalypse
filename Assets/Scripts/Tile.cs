using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string type;
    public GameObject highlight;

    private bool selected = false;
    private GameObject generatedHighlight;

    private int x;
    private int z;

    private float _height;
    public float height { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        type = name.Substring(0, name.Length - 7);
        height = transform.lossyScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseClick();
    }

    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit))
            {
                if (!selected && hit.transform.gameObject == this.gameObject)
                {
                    generatedHighlight = Instantiate(highlight, transform.position + new Vector3(0f, transform.lossyScale.y/2 + 0.1f, 0f), Quaternion.identity, null);
                    selected = !selected;
                    tag = "TileSelected";
                    EventManager.TriggerEvent("TileClickEvent");
                }
                else if (selected)
                {
                    selected = !selected;
                    tag = "TileDeselected";
                    Destroy(generatedHighlight);
                }
            }
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

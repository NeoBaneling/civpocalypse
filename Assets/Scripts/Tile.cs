using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string type;
    public GameObject highlight;

    private bool selected = false;
    private GameObject generatedHighlight;
    // Start is called before the first frame update
    void Start()
    {
        type = name;
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
                }
                else if (selected)
                {
                    selected = !selected;
                    Destroy(generatedHighlight);
                }
            }
        }
    }
}

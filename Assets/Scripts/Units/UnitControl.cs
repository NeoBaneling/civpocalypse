using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    public string name;
    public int speed;
    public int attack;
    public int defense;

    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                    selected = !selected;
                }
                else if (selected)
                {
                    // We're going to check if the unit can travel here
                }
            }
        }
    }
}

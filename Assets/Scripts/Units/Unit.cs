using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string name;
    public int speed;
    public int attack;
    public int defense;

    public Material baseM;
    public Material selectedM;

    [SerializeField]
    private string _faction;
    public string faction { get; set; }
    private int x;
    private int z;

    [SerializeField]
    private bool _isSelected;
    public bool isSelected { get; set; }
    private bool canMove = false;
    private List<GameObject> path = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        SetUnitToTile(GameBoardManager.Instance.GetTile((int)transform.position.x/3, (int)transform.position.z/3));
        isSelected = false;
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseClick();
        if (canMove)
        {
            TryToMoveUnit();
        }
    }

    // A creation function to set up all necessary parameters for a new Unit
    public void Setup(string faction, int x, int z, GameObject tile, bool isSelected)
    {
        this.faction = faction;
        this.isSelected = isSelected;
        SetCoords(x, z);
        SetUnitToTile(tile);
        baseM = (Material)Resources.Load("Materials/"+faction, typeof (Material));
        GetComponent<MeshRenderer>().material = baseM;
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

    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit))
            {
                if (!isSelected && hit.transform.gameObject == this.gameObject)
                {
                    isSelected = !isSelected;
                    GetComponent<MeshRenderer>().material = selectedM;
                    tag = "UnitSelected";
                    EventManager.TriggerEvent("UnitClickEvent");
                }
                else if (isSelected)
                {
                    if (GameBoardManager.Instance.selectedTile)
                    {
                        canMove = true;
                    }
                    isSelected = !isSelected;
                    GetComponent<MeshRenderer>().material = baseM;
                    tag = "UnitDeselected";
                }
            }
        }
    }

    // Resets the unit's transform to match the tile being moved to
    // Mostly needed to reset the height of the unit
    private void SetUnitToTile(GameObject tile)
    {
        int[] arr = tile.GetComponent<Tile>().GetCoords();
        SetCoords(arr[0], arr[1]);
        transform.position = new Vector3(arr[0]*3, tile.GetComponent<Tile>().height + transform.lossyScale.y, arr[1]*3);
    }

    private void TryToMoveUnit()
    {
        GameObject selectedTile = GameBoardManager.Instance.selectedTile;
        int[] arr = selectedTile.GetComponent<Tile>().GetCoords();
        string type = selectedTile.GetComponent<Tile>().type;
        if (type == "Mountain" || type == "Water") return;
        if (System.Math.Abs(arr[0] - x) > speed || System.Math.Abs(arr[1] - z) > speed) return;

        path = GameBoardManager.Instance.GetPath(x,z,arr[0],arr[1]);
        canMove = false;
        StartCoroutine("MoveToTile");
        /*
        if (speed > 1)
        {
            if (IsValidMovement(arr[0], arr[1]))
            {
                SetUnitToTile(selectedTile);
                EventManager.TriggerEvent("UnitMoveEvent");
                canMove = false;
            }
        }
        else
        {
            if (IsValidMovement(arr[0], arr[1]))
            {
                SetUnitToTile(selectedTile);
                EventManager.TriggerEvent("UnitMoveEvent");
                canMove = false;
            }
        }
        */
    }

    private bool IsValidMovement(int endX, int endZ)
    {
        // The positions for units (and everything else) are scaled by a factor of 3
        // due to the grid size, so here they have to be scaled back down to match
        // the position of the selected tile and the unit's speed.
        return System.Math.Abs(endX - transform.position.x/3) <= speed && System.Math.Abs(endZ - transform.position.z/3) <= speed;
    }

    // Temporary element to watch units move along paths
    private IEnumerator MoveToTile()
    {
        foreach (GameObject tile in path)
        {
            SetUnitToTile(tile);
            EventManager.TriggerEvent("UnitMoveEvent");
            yield return new WaitForSeconds(0.1f);
        }
    }
}

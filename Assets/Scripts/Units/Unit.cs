using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Unit stats
    public string unitName;
    public int speed;
    public int attack;
    public int defense;

    public Material baseM;
    public Material selectedM;

    [SerializeField]
    private Faction _faction;
    public Faction faction { get; set; }
    private int x;
    private int z;

    [SerializeField]
    private bool _isSelected;
    public bool isSelected { get; set; }
    private bool checkForTileClick = false;
    private bool canMove = false;
    private int currSteps = 0;
    private bool s_IsActive;
    public bool IsActive { get; set; }
    // A set of movements that detail how to get from point A to point B
    private Queue<GameObject> path = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetUnitToTile(GameBoardManager.Instance.GetTile((int)transform.position.x/3, (int)transform.position.z/3));
        isSelected = false;
        IsActive = true;
    }

    void OnEnable()
    {
        EventManager.StartListening("MoveUnitEvent", MoveUnit);
    }

    void OnDisable()
    {
        EventManager.StopListening("MoveUnitEvent", MoveUnit);
    }

    // Update is called once per frame
    void Update()
    {
        if (checkForTileClick)
        {
            CheckMouseClick();
        }
        if (canMove)
        {
            TryToMoveUnit();
        }
    }

    // A creation function to set up all necessary parameters for a new Unit
    public void Setup(Faction faction, int x, int z, GameObject tile, bool isSelected, string name)
    {
        this.faction = faction;
        this.isSelected = isSelected;
        this.unitName = name;
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

    // Just switches whether the unit is selected
    public void ToggleIsSelected()
    {
        Debug.Log(this.gameObject+" has been hit with a toggle");
        SetIsSelected(!isSelected);
    }

    // Handles all the flipping of schtuff when a unit is selected
    private void SetIsSelected(bool value)
    {
        isSelected = value;
        if (isSelected)
        {
            Debug.Log("We are selected");
            GetComponent<MeshRenderer>().material = selectedM;
            tag = "UnitSelected";
            EventManager.TriggerEvent("UnitSelectedEvent");
        }
        else
        {
            Debug.Log("No longer selected");
            GetComponent<MeshRenderer>().material = baseM;
            tag = "UnitDeselected";
        }
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
                    ToggleIsSelected();
                }
                else if (isSelected)
                {
                    if (GameBoardManager.Instance.selectedTile)
                    {
                        canMove = true;
                        checkForTileClick = false;
                    }
                    ToggleIsSelected();
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

    // Gets the selected tile for the unit to move to, and then asks the game board
    // manager to build a path from the unit's current position to the selected tile
    //
    // If no path is returned from the game board manager, we just don't do anything
    private void TryToMoveUnit()
    {
        GameObject selectedTile = GameBoardManager.Instance.selectedTile;
        int[] arr = selectedTile.GetComponent<Tile>().GetCoords();
        string type = selectedTile.GetComponent<Tile>().type;
        if (type == "Mountain" || type == "Water") return;

        path = GameBoardManager.Instance.GetPath(x,z,arr[0],arr[1]);
        canMove = false;
        if (path != null)
        {
            StartCoroutine("MoveToTile");
        }
    }

    private bool IsValidMovement(int endX, int endZ)
    {
        // The positions for units (and everything else) are scaled by a factor of 3
        // due to the grid size, so here they have to be scaled back down to match
        // the position of the selected tile and the unit's speed.
        return System.Math.Abs(endX - transform.position.x/3) <= speed && System.Math.Abs(endZ - transform.position.z/3) <= speed;
    }

    // Moves the unit utilizing the built path a limited number of steps
    private IEnumerator MoveToTile()
    {
        while (currSteps < speed && path.Count > 0)
        {
            SetUnitToTile(path.Dequeue());
            currSteps++;
            EventManager.TriggerEvent("UnitMovedEvent");
            // Temporary element to watch units move along paths
            yield return new WaitForSeconds(0.2f);
        }

        // We have exhausted all possible moves to get to our goal
        if (currSteps == speed)
        {
            currSteps = 0;
            SetIsSelected(false);
            yield return new WaitForSeconds(0.2f);
            Debug.Log("Triggering Unit Finished Moving Event");
            EventManager.TriggerEvent("UnitFinishedMovingEvent");
        }
        // We still have some steps left and can move one more space
        else
        {
            SetIsSelected(true);
            checkForTileClick = true;
        }
    }

    private void MoveUnit()
    {
        if (path.Count > 0 && isSelected)
        {
            StartCoroutine("MoveToTile");
        }
        else
        {
            checkForTileClick = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : GamePiece
{
    // Unit stats
    public string UnitName;
    public int Speed;
    public int Attack;
    public int Defense;
    public int Cost;

    // Movement variables
    private bool canMove = false;
    private int currSteps = 0;
    // A set of movements that detail how to get from point A to point B
    private Queue<GameObject> path = new Queue<GameObject>();

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

    public void Setup(Faction faction, Vector2Int coords, GameObject tile, bool isSelected, string name)
    {
        UnitName = name;
        Setup(faction, coords, tile, isSelected);
    }

    protected override void SetPieceToTile(GameObject tile)
    {
        Coords = tile.GetComponent<Tile>().Coords;
        transform.position = new Vector3(Coords[0]*3, tile.GetComponent<Tile>().height + (transform.lossyScale.y / 2), Coords[1]*3);
    }


    protected override void SetIsSelected(bool value)
    {
        IsSelected = value;
        if (IsSelected)
        {
            GetComponent<MeshRenderer>().material = SelectedMaterial;
            tag = "UnitSelected";
            EventManager.TriggerEvent("UnitSelectedEvent");
        }
        else
        {
            GetComponent<MeshRenderer>().material = Material;
            tag = "UnitDeselected";
        }
    }

    protected override void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit))
            {
                if (!IsSelected && hit.transform.gameObject == this.gameObject)
                {
                    ToggleIsSelected();
                }
                else if (IsSelected)
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

    // Gets the selected tile for the unit to move to, and then asks the game board
    // manager to build a path from the unit's current position to the selected tile
    //
    // If no path is returned from the game board manager, we just don't do anything
    private void TryToMoveUnit()
    {
        GameObject selectedTile = GameBoardManager.Instance.selectedTile;
        Vector2Int tileCoords = selectedTile.GetComponent<Tile>().Coords;
        string type = selectedTile.GetComponent<Tile>().type;
        if (type == "Mountain" || type == "Water") return;

        path = GameBoardManager.Instance.GetPath(Coords, tileCoords);
        canMove = false;
        if (path != null)
        {
            Debug.Log(path.Count);
            StartCoroutine("MoveToTile");
        }
    }

    private bool IsValidMovement(int endX, int endZ)
    {
        // The positions for units (and everything else) are scaled by a factor of 3
        // due to the grid size, so here they have to be scaled back down to match
        // the position of the selected tile and the unit's Speed.
        return System.Math.Abs(endX - transform.position.x/3) <= Speed && System.Math.Abs(endZ - transform.position.z/3) <= Speed;
    }

    // Moves the unit utilizing the built path a limited number of steps
    private IEnumerator MoveToTile()
    {
        while (currSteps < Speed && path.Count > 0)
        {
            SetPieceToTile(path.Dequeue());
            currSteps++;
            EventManager.TriggerEvent("UnitMovedEvent");
            // Temporary element to watch units move along paths
            yield return new WaitForSeconds(0.2f);
        }

        // We have exhausted all possible moves to get to our goal
        if (currSteps == Speed)
        {
            currSteps = 0;
            SetIsSelected(false);
            yield return new WaitForSeconds(0.2f);
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
        if (path.Count > 0 && IsSelected)
        {
            StartCoroutine("MoveToTile");
        }
        else
        {
            checkForTileClick = true;
        }
    }
}

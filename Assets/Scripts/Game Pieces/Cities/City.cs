using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : GamePiece
{
    // Each of these provides the number produced PER TURN
    public int Production { get; protected set; }
    public int BioMass { get; protected set; }
    public int Caps { get; protected set; }
    public int Lore { get; protected set; }

    public string ObjInProduction { get; protected set; }
    private int productionSpent;
    private bool currentlyProducing;
    private Dictionary<string, bool> validObjects = new Dictionary<string, bool>();

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Awake()
    {
        validObjects["BasicUnit"] = true;
        validObjects["Settler"] = true;
        TryToProduce("BasicUnit");
    }

    void Update()
    {

    }

    protected override void SetPieceToTile(GameObject tile)
    {
        Coords = tile.GetComponent<Tile>().Coords;
        transform.position = new Vector3(Coords[0]*3, tile.GetComponent<Tile>().height, Coords[1]*3);
    }

    protected override void SetIsSelected(bool value)
    {
        IsSelected = value;
        if (IsSelected)
        {
            GetComponent<MeshRenderer>().material = SelectedMaterial;
            tag = "CitySelected";
            EventManager.TriggerEvent("CitySelectedEvent");
            if (currentlyProducing)
            {
                IncrementProduction();
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material = Material;
            tag = "CityDeselected";
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
                        checkForTileClick = false;
                    }
                    ToggleIsSelected();
                }
            }
        }
    }

    private void TryToProduce(string objToProduce)
    {
        if (validObjects[objToProduce])
        {
            currentlyProducing = true;
        }
    }

    private void IncrementProduction()
    {
        productionSpent += Production;
        if (productionSpent >= 10) // Hardcoded value for production necessary.
        {
            UnitManager.Instance.CreateUnit("BasicUnit", Coords, Faction);
            productionSpent = 0;
            FinishProduction();
        }
        else
        {
            Debug.Log(productionSpent + " production spent on " + ObjInProduction);
            EventManager.TriggerEvent("CityFinishedTurnEvent");
            SetIsSelected(false);
        }
    }

    private void FinishProduction()
    {
        ObjInProduction = null;
        currentlyProducing = false;
    }
}

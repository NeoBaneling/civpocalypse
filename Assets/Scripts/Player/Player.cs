using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Faction faction = Faction.MYTHICAL;
    private GameObject[,] boardTiles;

    // Unit variables
    private List<GameObject> units;
    private Queue<GameObject> activeUnits;
    private bool hasActiveUnits;
    private GameObject currActiveUnit;

    // City variables
    private List<GameObject> cities;
    private Queue<GameObject> activeCities;
    private bool hasActiveCities;
    private GameObject currActiveCity;

    void Start()
    {
        InstantiatePlayer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && currActiveUnit.GetComponent<Unit>().unitName.Equals("Settler"))
        {
            CreateCity();
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("UnitMovedEvent", RemoveFogWithUnit);
        EventManager.StartListening("TurnStartEvent", BeginTurn);
        EventManager.StartListening("UnitFinishedMovingEvent", ToNextUnit);
    }

    void OnDisable()
    {
        EventManager.StopListening("UnitMovedEvent", RemoveFogWithUnit);
        EventManager.StopListening("TurnStartEvent", BeginTurn);
        EventManager.StopListening("UnitFinishedMovingEvent", ToNextUnit);
    }

    public void SetActiveUnits(Queue<GameObject> activeUnits)
    {
        this.activeUnits = activeUnits;
    }

    /**
     * Provides the player with everything they need at the beginning of the game
     **/
    private void InstantiatePlayer()
    {
        units = new List<GameObject>();
        cities = new List<GameObject>();
        activeUnits = new Queue<GameObject>();
        hasActiveUnits = false;
        hasActiveCities = false;
        currActiveUnit = null;
        currActiveCity = null;
        CreateBoardTiles();
        // CreateBasicUnit();
        CreateBasicUnit();
        CreateBasicSpeedyUnit();
        CreateSettlers();
    }

    /**
     * Creates a new array of board tiles, but populates them all with fog
     **/
    private void CreateBoardTiles()
    {
        int boardSize = GameBoardManager.Instance.boardSize;
        boardTiles = new GameObject[boardSize, boardSize];
        GameObject[,] gameBoardTiles = GameBoardManager.Instance.GetBoard();
        GameObject fog = GameBoardManager.Instance.GetFog();
        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                int[] tileCoords = gameBoardTiles[x,z].GetComponent<Tile>().GetCoords();
                GameObject newFog = Instantiate(fog, new Vector3(0, 0, 0), Quaternion.identity, null);
                newFog.transform.position = newFog.transform.position + new Vector3(tileCoords[0]*3, newFog.transform.lossyScale.y/2, tileCoords[1]*3);
                boardTiles[x,z] = newFog;
            }
        }
    }

    /**
     * Destroys any fog that is considered in a "discovered" area (5*5 radius)
     * and repopulates it with whatever is stored in the GameBoardManager
     **/
    private void RemoveFog(int x, int z)
    {
        int boardSize = GameBoardManager.Instance.boardSize;
        for (int areaX = x - 2; areaX <= x + 2; areaX++)
        {
            for (int areaZ = z - 2; areaZ <= z + 2; areaZ++)
            {
                if (areaZ >= 0 && areaZ < boardSize && boardTiles[(areaX%boardSize + boardSize)%boardSize, areaZ].GetComponent<Tile>().type == "Fog")
                {
                    Destroy(boardTiles[(areaX%boardSize + boardSize)%boardSize, areaZ]);
                    boardTiles[(areaX%boardSize + boardSize)%boardSize, areaZ] = GameBoardManager.Instance.GetTile(areaX, areaZ);
                }
            }
        }
    }

    private void RemoveFogWithUnit()
    {
        int[] coords = currActiveUnit.GetComponent<Unit>().GetCoords();
        RemoveFog(coords[0], coords[1]);
    }

    private void BeginTurn()
    {
        ToNextUnit();
    }

    private void ToNextUnit()
    {
        Debug.Log("It's time to get another unit");
        if (activeUnits.Count > 0)
        {
            currActiveUnit = activeUnits.Dequeue();
            Debug.Log(currActiveUnit);
            hasActiveUnits = true;
            currActiveUnit.GetComponent<Unit>().ToggleIsSelected();
        }
        else
        {
            hasActiveUnits = false;
            EventManager.TriggerEvent("TurnCompleteEvent");
        }
    }

    private void CreateCity()
    {
        int[] settlerCoords = currActiveUnit.GetComponent<Unit>().GetCoords();
        int x = settlerCoords[0];
        int z = settlerCoords[1];

        for (int i = 0; i < units.Count; i++)
        {
            GameObject unit = units[i];
            Unit unitInfo = unit.GetComponent<Unit>();
            int[] coords = unitInfo.GetCoords();
            if (unitInfo.unitName.Equals("Settler") && coords[0] == x && coords[1] == z)
            {
                Destroy(unit);
                units[i] = null;
                break;
            }
        }
        cities.Add(CityManager.Instance.CreateCity(x, z, faction));
        EventManager.TriggerEvent("UnitFinishedMovingEvent");
        UnitManager.Instance.DestroyUnit("Settler", x, z, faction);
    }

    ///////////////////////////////
    //
    //
    //  BEGIN UNIT INSTANTIATION METHODS
    //
    //
    ///////////////////////////////

    /**
     * Finds an adequate spot for the unit to exist, and then creates them.
     **/
    private void CreateBasicUnit()
    {
        int x = 0;
        int z = 0;
        do
        {
            // +- 2 cause we don't want to end up on ice
            x = Random.Range(0, GameBoardManager.Instance.boardSize);
            z = Random.Range(2, GameBoardManager.Instance.boardSize - 2);
        } while (GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Mountain" || GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Water");

        units.Add(UnitManager.Instance.CreateUnit("BasicUnit", x, z, faction));
        EventManager.TriggerEvent("UnitGeneratedEvent");
        RemoveFog(x, z);
    }

    /**
     * Finds an adequate spot for the unit to exist, and then creates them.
     **/
    private void CreateBasicSpeedyUnit()
    {
        int x = 0;
        int z = 0;
        do
        {
            // +- 2 cause we don't want to end up on ice
            x = Random.Range(0, GameBoardManager.Instance.boardSize);
            z = Random.Range(2, GameBoardManager.Instance.boardSize - 2);
        } while (GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Mountain" || GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Water");

        units.Add(UnitManager.Instance.CreateUnit("BasicSpeedyUnit", x, z, faction));
        EventManager.TriggerEvent("UnitGeneratedEvent");
        RemoveFog(x, z);
    }

    /**
     * Finds an adequate spot for settlers to exist, and then creates them.
     **/
    private void CreateSettlers()
    {
        int x = 0;
        int z = 0;
        do
        {
            // +- 2 cause we don't want to end up on ice
            x = Random.Range(2, GameBoardManager.Instance.boardSize - 2);
            z = Random.Range(0, GameBoardManager.Instance.boardSize);
        } while (GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Mountain" || GameBoardManager.Instance.GetTile(x, z).GetComponent<Tile>().type == "Water");

        units.Add(UnitManager.Instance.CreateUnit("Settler", x, z, faction));
        EventManager.TriggerEvent("UnitGeneratedEvent");
        RemoveFog(x, z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitManager s_Instance;
    public static UnitManager Instance
    {
        get {return s_Instance;}
    }

    private List<GameObject>[] units;

    [SerializeField]
    private GameObject _selectedUnit;
    public GameObject selectedUnit { get; set; }

    void Awake()
    {
        // This handles the GameBoardManager instance
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else if (s_Instance != this)
        {
            throw new UnityException("There cannot be more than one UnitManager script. The instances are " + s_Instance.name + " and " + name + ".");
        }
    }

    void Start()
    {
        units = new List<GameObject>[GameBoardManager.Instance.boardSize*GameBoardManager.Instance.boardSize];
    }

    void OnEnable()
    {
        EventManager.StartListening("UnitSelectedEvent", HighlightUnit);
        EventManager.StartListening("UnitFinishedMovingEvent", DehilightUnit);
    }

    void OnDisable()
    {
        EventManager.StopListening("UnitSelectedEvent", HighlightUnit);
        EventManager.StopListening("UnitFinishedMovingEvent", DehilightUnit);
    }

    /**
     * Instantiates a unit with the specified string name and player name.
     * Requires a valid unit name to work successfully.
     **/
    public GameObject CreateUnit(string name, int x, int z, Faction playerFaction)
    {
        GameObject unit = (GameObject) Instantiate(Resources.Load("Prefabs/Units/"+name), new Vector3(0, 0, 0), Quaternion.identity, null);
        unit.GetComponent<Unit>().Setup(playerFaction, x, z, GameBoardManager.Instance.GetTile(x, z), true);
        selectedUnit = unit;
        if (units[0] == null)
        {
            units[0] = new List<GameObject>();
        }
        units[0].Add(unit);
        return unit;
    }

    /**
     * Cycles through all active units (i.e. not defending) and populates five
     * different lists (one for each faction). Then, it gives each list to the
     * appropriate faction.
     *
     * Currently only works for the mythical list, for the player.
     **/
    public void GatherActiveUnits()
    {
        Queue<GameObject> mythicalUnits = new Queue<GameObject>();
        Queue<GameObject> mutantUnits = new Queue<GameObject>();
        Queue<GameObject> evolvedUnits = new Queue<GameObject>();
        Queue<GameObject> zombieUnits = new Queue<GameObject>();
        Queue<GameObject> robotUnits = new Queue<GameObject>();

        foreach (GameObject unit in units[0])
        {
            if (unit.GetComponent<Unit>().IsActive)
            {
                Faction faction = unit.GetComponent<Unit>().faction;
                if (faction == Faction.MYTHICAL)
                {
                    mythicalUnits.Enqueue(unit);
                }
                if (faction == Faction.MUTANT)
                {
                    mutantUnits.Enqueue(unit);
                }
                if (faction == Faction.EVOLVED)
                {
                    evolvedUnits.Enqueue(unit);
                }
                if (faction == Faction.ZOMBIE)
                {
                    zombieUnits.Enqueue(unit);
                }
                if (faction == Faction.ROBOT)
                {
                    robotUnits.Enqueue(unit);
                }
            }
        }

        GameObject.Find("Player").GetComponent<Player>().SetActiveUnits(mythicalUnits);
    }

    private void HighlightUnit()
    {
        Debug.Log("We're highlighting the unit");
        GameObject unit = GameObject.FindWithTag("UnitSelected");
        selectedUnit = unit;
    }

    private void DehilightUnit()
    {
        Debug.Log("We're dehighlighting the unit");
        selectedUnit = null;
    }
}

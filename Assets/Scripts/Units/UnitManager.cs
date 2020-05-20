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

        units = new List<GameObject>[GameBoardManager.Instance.boardSize*GameBoardManager.Instance.boardSize];
    }

    /**
     * Instantiates a unit with the specified string name and player name.
     * Requires a valid unit name to work successfully.
     **/
    public GameObject CreateUnit(string name, int x, int y, int z, string player)
    {
        GameObject unit = (GameObject) Instantiate(Resources.Load("Prefabs/Units/"+name), new Vector3(x, y, z), Quaternion.identity, null);
        unit.GetComponent<Unit>().SetFaction(player);
        if (units[0] == null)
        {
            units[0] = new List<GameObject>();
        }
        units[0].Add(unit);
        return unit;
    }
}

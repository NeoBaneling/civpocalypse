using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    private static CityManager s_Instance;
    public static CityManager Instance
    {
        get { return s_Instance; }
    }

    // Each index into the list is a Faction that provides a list of all
    // cities owned by that faction
    private Dictionary<Faction, List<GameObject>> cities;

    [SerializeField]
    private GameObject _selectedCity;
    public GameObject selectedCity { get; set; }

    void Awake()
    {
        // This handles the CityManager instance
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else if (s_Instance != this)
        {
            throw new UnityException("There cannot be more than one CityManager script. The instances are " + s_Instance.name + " and " + name + ".");
        }
    }

    void Start()
    {
        cities = new Dictionary<Faction, List<GameObject>>();
        // Initalize all of the lists in cities
        cities[Faction.MYTHICAL] = new List<GameObject>();
        cities[Faction.MUTANT] = new List<GameObject>();
        cities[Faction.EVOLVED] = new List<GameObject>();
        cities[Faction.ZOMBIE] = new List<GameObject>();
        cities[Faction.ROBOT] = new List<GameObject>();
    }

    /**
     * Instantiates a city with the specified player name.
     **/
    public GameObject CreateCity(int x, int z, Faction playerFaction)
    {
        GameObject city = (GameObject) Instantiate(Resources.Load("Prefabs/City"), new Vector3(0, 0, 0), Quaternion.identity, null);
        city.GetComponent<City>().Setup(playerFaction, x, z, GameBoardManager.Instance.GetTile(x, z), true);
        selectedCity = city;
        cities[playerFaction].Add(city);
        return city;
    }
}

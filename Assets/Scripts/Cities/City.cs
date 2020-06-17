using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    //City information
    public Material baseM;

    [SerializeField]
    private Faction _faction;
    public Faction faction { get; set; }
    private int x;
    private int z;

    [SerializeField]
    private bool _isSelected;
    public bool isSelected { get; set; }
    private bool checkForTileClick = false;
    private bool s_IsActive;
    public bool IsActive { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Faction faction, int x, int z, GameObject tile, bool isSelected)
    {
        this.faction = faction;
        this.isSelected = isSelected;
        SetCoords(x, z);
        SetCityToTile(tile);
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

    // Resets the city's transform to match the tile being set to
    // Mostly needed to set the height of the city
    private void SetCityToTile(GameObject tile)
    {
        int[] arr = tile.GetComponent<Tile>().GetCoords();
        SetCoords(arr[0], arr[1]);
        transform.position = new Vector3(arr[0]*3, tile.GetComponent<Tile>().height + (transform.lossyScale.y / 2), arr[1]*3);
    }
}

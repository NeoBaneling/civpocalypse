using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    private static GameBoardManager s_Instance;
    public static GameBoardManager Instance
    {
        get {return s_Instance;}
    }

    [SerializeField] // Not 100% sure on what this does but "borrowed" from
                     // http://johnstejskal.com/wp/getters-setters-and-auto-properties-in-c-explained-get-set/
    private int _boardSize;
    public int boardSize { get; set; }

    public GameObject gameBoard;

    private GameObject[] tiles;
    private GameObject _selectedTile;
    public GameObject selectedTile { get; set; }

    void Awake()
    {
        // This handles the GameBoardManager instance
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else if (s_Instance != this)
        {
            throw new UnityException("There cannot be more than one GameBoardManager script. The instances are " + s_Instance.name + " and " + name + ".");
        }

        boardSize = 15;
        tiles = new GameObject[boardSize * boardSize];
        gameBoard.GetComponent<GameBoardPopulator>().GenerateGameBoard();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        EventManager.StartListening("TileClickEvent", HighlightTile);
    }

    void OnDisable()
    {
        EventManager.StopListening("TileClickEvent", HighlightTile);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTile(GameObject tile, int x, int z)
    {
        tiles[z + (boardSize * x)] = tile;
    }

    public GameObject GetTile(int x, int z)
    {
        try
        {
            return tiles[z + (boardSize * x)];
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogError("The location " + x + ", " + z + " is not valid.");
            return null;
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("Tile could not be found at location " + x + ", " + z + ".");
            return null;
        }
        catch (UnityException e)
        {
            Debug.LogError("An exception occured.");
            return null;
        }
    }

    void HighlightTile()
    {
        GameObject tile = GameObject.FindGameObjectsWithTag("TileSelected")[0];
        int[] arr = tile.GetComponent<Tile>().GetCoords();
        selectedTile = GetTile(arr[0], arr[1]);
    }
}
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
    public int boardSize
    {
        get {return 30;}
    }

    public GameObject gameBoard;
    public GameObject fog;

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

        tiles = new GameObject[boardSize * boardSize];
        gameBoard.GetComponent<GameBoardPopulator>().GenerateGameBoard();
    }

    void OnEnable()
    {
        EventManager.StartListening("TileClickEvent", HighlightTile);
    }

    void OnDisable()
    {
        EventManager.StopListening("TileClickEvent", HighlightTile);
    }

    public void SetTile(GameObject tile, int x, int z)
    {
        if (tiles[x + boardSize * z] != null)
        {
            Destroy(tiles[x + boardSize * z]);
        }
        tiles[x + (boardSize * z)] = tile;
    }

    public GameObject GetTile(int x, int z)
    {
        try
        {
            return tiles[x + (boardSize * z)];
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

    public List<GameObject> GetNeighboringTiles(int x, int z)
    {
        List<GameObject> list = new List<GameObject>();
        bool canGoLeft = x != 0;
        bool canGoRight = x != boardSize - 1;
        bool canGoDown = z != 0;
        bool canGoUp = z != boardSize - 1;

        if (canGoLeft)
        {
            list.Add(GetTile(x - 1, z));
        }
        if (canGoRight)
        {
            list.Add(GetTile(x + 1, z));
        }
        if (canGoDown)
        {
            list.Add(GetTile(x, z - 1));
        }
        if (canGoUp)
        {
            list.Add(GetTile(x, z + 1));
        }
        if (canGoLeft && canGoUp)
        {
            list.Add(GetTile(x - 1, z + 1));
        }
        if (canGoLeft && canGoDown)
        {
            list.Add(GetTile(x - 1, z - 1));
        }
        if (canGoRight && canGoUp)
        {
            list.Add(GetTile(x + 1, z + 1));
        }
        if (canGoRight && canGoDown)
        {
            list.Add(GetTile(x + 1, z - 1));
        }
        return list;
    }

    public GameObject GetFog()
    {
        return fog;
    }

    public GameObject[] GetBoard()
    {
        return tiles;
    }

    void HighlightTile()
    {
        GameObject tile = GameObject.FindGameObjectsWithTag("TileSelected")[0];
        int[] arr = tile.GetComponent<Tile>().GetCoords();
        selectedTile = GetTile(arr[0], arr[1]);
    }
}
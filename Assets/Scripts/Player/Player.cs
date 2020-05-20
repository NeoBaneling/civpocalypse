using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string faction = "Mythical";
    private GameObject[,] boardTiles;
    private List<GameObject> units;

    void Start()
    {
        InstantiatePlayer();
    }

    void OnEnable()
    {
        EventManager.StartListening("UnitMoveEvent", RemoveFogWithUnit);
    }

    void OnDisable()
    {
        EventManager.StopListening("UnitMoveEvent", RemoveFogWithUnit);
    }

    /**
     * Provides the player with everything they need at the beginning of the game
     **/
    private void InstantiatePlayer()
    {
        units = new List<GameObject>();
        CreateBoardTiles();
        CreateBasicUnit();
        // CreateSettlers();
        RemoveFog(units[0].GetComponent<Unit>().GetCoords()[0], units[0].GetComponent<Unit>().GetCoords()[1]);
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

    /**
     * Finds an adequate spot for settlers to exist, and then creates them.
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
    }

    void RemoveFogWithUnit()
    {
        GameObject unit = UnitManager.Instance.selectedUnit;
        if (unit.GetComponent<Unit>().faction == this.faction)
        {
            int[] coords = unit.GetComponent<Unit>().GetCoords();
            Debug.Log(coords[0] +", "+coords[1]);
            RemoveFog(coords[0], coords[1]);
        }
    }
}

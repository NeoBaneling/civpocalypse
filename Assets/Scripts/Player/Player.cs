using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string faction;
    private GameObject[,] boardTiles;
    private GameObject[] units;

    void Start()
    {
        CreateBoardTiles();
    }

    /**
     * Creates a new array of board tiles, but populates them all with fog
     **/
    private void CreateBoardTiles()
    {
        int boardSize = GameBoardManager.Instance.boardSize;
        boardTiles = new GameObject[boardSize, boardSize];
        boardTiles = GameBoardManager.Instance.GetBoard();
        GameObject fog = GameBoardManager.Instance.GetFog();
        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                int[] tileCoords = boardTiles[x,z].GetComponent<Tile>().GetCoords();
                GameObject newFog = Instantiate(fog, new Vector3(0, 0, 0), Quaternion.identity, null);
                newFog.transform.position = newFog.transform.position + new Vector3(tileCoords[0]*3, newFog.transform.lossyScale.y/2, tileCoords[1]*3);
                boardTiles[x,z] = newFog;
            }
        }
    }
}

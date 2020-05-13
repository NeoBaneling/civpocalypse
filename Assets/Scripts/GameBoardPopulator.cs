using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardPopulator : MonoBehaviour
{
    // All of the prefabs we'll be using to populate our board
    public GameObject[] boardTiles;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This method randomly generates a game board for the game.
     * It cycles through each child of the game board gameObject,
     * which specify to each row of the board.
     * Then, for each child of the game board, it cycles through each column
     * and populates each square with a certain tile from the public boardTiles
     * array.
     * Tile generation is built off a function written below.
     **/
    public void GenerateGameBoard()
    {

        // The current row of the board grid, used for specifying Z coordinates
        int childZ = 0;
        int boardSize = GameBoardManager.Instance.boardSize;
        foreach (Transform child in transform)
        {
            for (int tileX = 0; tileX < boardSize; tileX++)
            {
                /**
                 *         if x < 2        | sqrt(-x + 1)^8 + 6
                 * f(x) =  if 2 <= x <= 12 | random(x)
                 *         if x > 12       | sqrt(x - 13)^8 + 6
                 *
                 * The first and third parts of the function controls the borders of the map -
                 * The ice and water
                 *
                 * The second part of the function adds in random spawn of all
                 * terrain types EXCLUDING ice
                 *
                 **/
                int bottomIndex = 0;
                int topIndex = 0;

                if (childZ < 2)
                {
                    double x = System.Math.Sqrt(System.Math.Abs(-1 * childZ + 1));
                    double xsqrd = System.Math.Pow(x, 8);
                    bottomIndex = System.Convert.ToInt32(xsqrd + 6);
                    topIndex = boardTiles.Length;
                }
                else if (childZ > boardSize - 3)
                {
                    double x = System.Math.Sqrt(System.Math.Abs(childZ - ((boardTiles.Length - 1) * 2 - 1)));
                    double xsqrd = System.Math.Pow(x, 8);
                    bottomIndex = System.Convert.ToInt32(xsqrd + 6);
                    topIndex = boardTiles.Length;
                }
                else
                {
                    bottomIndex = 0;
                    topIndex = boardTiles.Length - 1;
                }
                int tileIndex = Random.Range(bottomIndex, topIndex);
                // Debug.Log("Bottom Index: " + bottomIndex + ", Top Index: " + boardTiles.Length + ", Provided Index: " + tileIndex);
                GameObject tile = Instantiate(boardTiles[tileIndex], new Vector3(0, 0, 0), Quaternion.identity, null);
                tile.transform.position = child.position + new Vector3(tileX*3, tile.transform.lossyScale.y/2, 0);
                tile.GetComponent<Tile>().SetCoords(tileX, childZ);
                GameBoardManager.Instance.SetTile(tile, tileX, childZ);
            }
            childZ++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardPopulator : MonoBehaviour
{
    // All of the prefabs we'll be using to populate our board
    public GameObject[] boardTiles;

    /**
     * This method randomly generates a game board for the game.
     * It cycles through each child of the game board gameObject,
     * which specify to each row of the board.
     * Then, for each child of the game board, it cycles through each column
     * and populates each square with a certain tile from the public boardTiles
     * array.
     *
     * After populating each square, the game board does another pass over the
     * ice regions to make sure eac ice square is surrounded only by water or
     * ice, filling in unwanted squares with water
     *
     **/
    public void GenerateGameBoard()
    {
        int boardSize = GameBoardManager.Instance.boardSize;

        GenerateFirstPass(boardSize);
        GenerateSecondPass(boardSize);
    }

    private void GenerateFirstPass(int boardSize)
    {
        // The number of rows we want for areas that are only snow or water
        // (they come just before and just after ice)
        int tundraRowSize = (int) Mathf.Round(boardSize / 8.0f);
        // col corresponds to the X position, row corresponds to Z position
        for (int row = 0; row < boardSize; row++)
        {
            float[] weights = new float[boardTiles.Length];

            if (row == 0 || row == boardSize - 1)
            {
                weights[7] = 1.0f;
            }
            // When we are on the border of ice and water
            else if (row == 1 || row == boardSize-2)
            {
                weights[6] = 0.75f;
                weights[7] = 0.25f;
            }
            // If we're in the tundra region
            else if ((row > 1 && row < tundraRowSize + 2) || (row > boardSize - tundraRowSize - 3 && row < boardSize - 2))
            {
                weights[4] = 0.2f;
                weights[5] = 0.4f;
                weights[6] = 0.6f;
            }
            // Everything else
            else
            {
                weights[0] = 0.04f;
                weights[1] = 0.10f;
                weights[2] = 0.05f;
                weights[3] = 0.18f;
                weights[4] = 0.18f;
                weights[5] = 0.00f;
                weights[6] = 0.45f;
            }

            for (int col = 0; col < boardSize; col++)
            {
                int tileIndex = ReturnWeightedIndex(weights);
                GameObject tile = Instantiate(boardTiles[tileIndex], new Vector3(0, 0, 0), Quaternion.identity, null);
                tile.transform.position = tile.transform.position + new Vector3(col*3, tile.transform.lossyScale.y/2, row*3);
                tile.GetComponent<Tile>().SetCoords(col, row);
                GameBoardManager.Instance.SetTile(tile, col, row);
            }
        }
    }

    private void GenerateSecondPass(int boardSize)
    {
        // Looks a little janky, but basically gets us to the two rows with ice
        // and water
        for (int row = 1; row < boardSize; row*=boardSize-2)
        {
            for (int col = 0; col < boardSize; col++)
            {
                // Debug.Log(GameBoardManager.Instance.GetTile(col, row).GetComponent<Tile>().type);
                if (GameBoardManager.Instance.GetTile(col, row).GetComponent<Tile>().type == "Ice")
                {
                    foreach (GameObject oldTile in GameBoardManager.Instance.GetNeighboringTiles(col, row))
                    {
                        if (oldTile.GetComponent<Tile>().type != "Ice" && oldTile.GetComponent<Tile>().type != "Water")
                        {
                            int[] newPos = oldTile.GetComponent<Tile>().GetCoords();
                            GameObject newTile = Instantiate(boardTiles[6], new Vector3(0, 0, 0), Quaternion.identity, null);
                            newTile.transform.position = new Vector3(newPos[0]*3, newTile.transform.lossyScale.y/2, newPos[1]*3);
                            newTile.GetComponent<Tile>().SetCoords(newPos[0], newPos[1]);
                            GameBoardManager.Instance.SetTile(newTile, newPos[0], newPos[1]);
                        }
                    }
                }
            }
        }
    }

    private int ReturnWeightedIndex(float[] weights)
    {
        List<int> weightedIndeces = new List<int>();
        int currWeightedIndex = 0;
        int prevIndex = 0;
        foreach (float weight in weights)
        {
            int targetIndex = (int) Mathf.Round(weight*100);
            for (int i = 0; i < targetIndex; i++)
            {
                weightedIndeces.Add(currWeightedIndex);
            }
            currWeightedIndex++;
        }

        return weightedIndeces[Random.Range(0, 100)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A parent class for any "physical" piece of the game
 **/
public abstract class GamePiece : MonoBehaviour
{
    /**
     * Faction provides which faction owns the game piece.
     *
     * Coords provides an (x, z) Vector2 scaled with the game board. Must be scaled
     * by three to achieve real world coordinates.
     *
     * Material specifies the color of the game piece based on its faction.
     **/
    public Faction Faction { get; protected set; }
    public Vector2Int Coords { get; protected set; }
    public Material Material { get; protected set; }
    public Material SelectedMaterial { get; protected set; }

    public bool IsSelected { get; protected set; }
    public bool IsActive { get; protected set; }

    protected bool checkForTileClick = false;

    public void Setup(Faction faction, Vector2Int coords, GameObject tile, bool isSelected)
    {
        this.Faction = faction;
        this.Coords = coords;
        SetPieceToTile(tile);
        Material = (Material) Resources.Load("Materials/"+faction, typeof (Material));
        SelectedMaterial = (Material) Resources.Load("Materials/Highlight", typeof (Material));
        GetComponent<MeshRenderer>().material = Material;

        IsSelected = false;
        IsActive = true;
    }

    protected abstract void SetPieceToTile(GameObject tile);

    // Just switches whether the piece is selected
    public void ToggleIsSelected()
    {
        SetIsSelected(!IsSelected);
    }

    // Handles all the flipping of schtuff when a piece is selected
    protected abstract void SetIsSelected(bool value);

    protected abstract void CheckMouseClick();
}

using UnityEngine;
using System   ;

/// <summary>
/// Class represents a game participant.
/// </summary>
public class Player : MonoBehaviour
{
    public int PlayerNumber;
    public Color Color;

    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>         
    public void Play(TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
    }
}

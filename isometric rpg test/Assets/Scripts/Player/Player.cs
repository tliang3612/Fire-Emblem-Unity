using UnityEngine;
using System   ;

/// <summary>
/// Class represents a game participant.
/// </summary>
public abstract class Player : MonoBehaviour
{
    public int PlayerNumber;
    public Color Color;

    public virtual void Initialize(TileGrid cellGrid) { }
     
    public abstract void Play(TileGrid cellGrid);

}

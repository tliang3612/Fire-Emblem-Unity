using Unity.Mathematics;
using UnityEngine;

public class ArrowTranslator 
{
    public enum ArrowDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopLeft = 5,
        BottomLeft = 6,
        TopRight = 7,
        BottomRight = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }

    public ArrowDirection TranslateDirection(OverlayTile previousTile, OverlayTile currentTile, OverlayTile futureTile)
    {
        bool isFinal = futureTile == null;

        //if a previous tile doesnt exist, past direction is Vector2Int(0,0)
        Vector2Int pastDirection = previousTile != null ? currentTile.gridLocation2D - previousTile.gridLocation2D : new Vector2Int(0, 0);
        Vector2Int futureDirection = futureTile != null ? futureTile.gridLocation2D - currentTile.gridLocation2D : new Vector2Int(0, 0);
        Vector2Int direction = pastDirection != futureDirection ? futureDirection + pastDirection : futureDirection;

        switch (direction)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                return isFinal == false ? ArrowDirection.Up : ArrowDirection.UpFinished;
                 
            case Vector2Int v when v.Equals(Vector2Int.down):
                return isFinal == false ? ArrowDirection.Down : ArrowDirection.DownFinished;

            case Vector2Int v when v.Equals(Vector2Int.left):
                return isFinal == false ? ArrowDirection.Left : ArrowDirection.LeftFinished;

            case Vector2Int v when v.Equals(Vector2Int.right):
                return isFinal == false ? ArrowDirection.Right : ArrowDirection.RightFinished;

            case Vector2Int v when v.Equals(new Vector2Int(1, 1)):
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomRight : ArrowDirection.TopLeft;

            case Vector2Int v when v.Equals(new Vector2Int(-1, 1)):
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomLeft : ArrowDirection.TopRight;

            case Vector2Int v when v.Equals(new Vector2Int(1, -1)):
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomLeft : ArrowDirection.TopRight;

            case Vector2Int v when v.Equals(new Vector2Int(-1, -1)):
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomRight : ArrowDirection.TopLeft;

        }  

        return ArrowDirection.None;


    }
}

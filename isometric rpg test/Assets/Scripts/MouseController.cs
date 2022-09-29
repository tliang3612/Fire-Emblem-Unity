/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tiles;

public class MouseController : MonoBehaviour
{
    public float speed;
    public GameObject characterPrefab;
    private CharacterInfo characterInfo;
    private RangeFinder rangeFinder;
    private AStarPathfinder pathFinder;

    private List<OverlayTile> path;
    private List<OverlayTile> tilesInRange;
    private ArrowTranslator arrowTranslator;


    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new AStarPathfinder();
        rangeFinder = new RangeFinder();
        tilesInRange = new List<OverlayTile>();
        arrowTranslator = new ArrowTranslator();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit2D? hoveredTileHit = GetHoveredTileHit();

        if (hoveredTileHit.HasValue)
        {

            GameObject overlayTileObject = hoveredTileHit.Value.collider.gameObject;
            OverlayTile overlayTile = overlayTileObject.GetComponent<OverlayTile>();
            transform.position = overlayTileObject.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTileObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

            if (tilesInRange.Contains(overlayTile) && !isMoving && characterInfo != null)
            {
                path = pathFinder.FindPath(characterInfo.activeTile, overlayTile, tilesInRange);
                TranslateArrows();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (characterInfo == null)
                {
                    characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
                    PositionCharacter(overlayTile);
                    GetInRangeTiles();
                }
                else if(tilesInRange.Contains(overlayTile) && !isMoving)
                {
                    isMoving = true;
                }
            }

            if (path?.Count > 0 && isMoving)
            {
                MoveCharAlongPath();
            }
        }
    }

    private void GetInRangeTiles()
    {
        //hide the previous items from tilesInRange
        if(tilesInRange.Count != 0)
        {
            foreach (var item in tilesInRange)
            {
                item.HideTile();
            }
        }

        tilesInRange = rangeFinder.GetTilesInRange(characterInfo.activeTile, 3);

        foreach(var item in tilesInRange)
        {
            item.ShowTile();
        }

    }

    private void MoveCharAlongPath()
    {
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        characterInfo.transform.position = Vector2.MoveTowards(characterInfo.transform.position, path[0].transform.position, step);
        //move towards z
        characterInfo.transform.position = new Vector3(characterInfo.transform.position.x, characterInfo.transform.position.y, zIndex);

        if(Vector2.Distance(characterInfo.transform.position, path[0].transform.position) < 0.001f)
        {
            PositionCharacter(path[0]);
            path.RemoveAt(0);
        }

        if(path.Count == 0)
        {
            GetInRangeTiles();
            isMoving = false;
        }
    }

    public RaycastHit2D? GetHoveredTileHit()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //returns an array of raycast hits 
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        if(hits.Length > 0)
        {
            //returns the first collider hit's transform.position.z
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }

    private void PositionCharacter(OverlayTile overlayTile)
    {
        characterInfo.transform.position = new Vector3(overlayTile.transform.position.x, overlayTile.transform.position.y, overlayTile.transform.position.z);
        characterInfo.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        characterInfo.activeTile = overlayTile;
    }

    private void TranslateArrows()
    {
        foreach (var item in tilesInRange)
        {
            item.SetArrowSprite(ArrowTranslator.ArrowDirection.None);
        }

        for (int i = 0; i < path.Count; i++)
        {
            var previousTile = i > 0 ? path[i - 1] : characterInfo.activeTile;
            var futureTile = i < path.Count - 1 ? path[i + 1] : null;

            var arrow = arrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
            path[i].SetArrowSprite(arrow);
        }
    }

}
*/
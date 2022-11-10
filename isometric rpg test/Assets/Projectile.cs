using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    private float targetPositionX;
    private float directionX;
    public float speed;

    public void SetProperties(float startPosX, float posX, float direction)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosX, GetComponent<RectTransform>().anchoredPosition.y);
        GetComponent<RectTransform>().localScale = new Vector3(direction, 1, 1);
        targetPositionX = posX;        
        directionX = direction;
    }

    public IEnumerator MoveProjectile()
    {        
        while (directionX * GetComponent<RectTransform>().anchoredPosition.x > directionX * targetPositionX)
        {
            GetComponent<RectTransform>().Translate(Vector3.left * directionX * Time.deltaTime * speed);
            yield return null;
            
        }
        Destroy(gameObject);
    }
}

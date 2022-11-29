using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    private float targetPositionX;
    private float directionX;
    private RectTransform rectTransform;
    [SerializeField] private float speed;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetProperties(float startPosX, float posX, float direction)
    {
        rectTransform.anchoredPosition.Set(startPosX, rectTransform.anchoredPosition.y);
        rectTransform.localScale.Set(direction, 1, 1);
        targetPositionX = posX;        
        directionX = direction;
        
    }

    public IEnumerator MoveProjectile()
    {        
        while (directionX * rectTransform.anchoredPosition.x > directionX * targetPositionX)
        {
            rectTransform.Translate(Vector3.left * directionX * Time.deltaTime * speed);
            yield return null;
            
        }
        Destroy(gameObject);
    }
}

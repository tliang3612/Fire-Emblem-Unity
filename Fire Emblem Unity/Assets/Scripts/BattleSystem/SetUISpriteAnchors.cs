using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetUISpriteAnchors : MonoBehaviour
{
    //Updates the Image pivot to be relative to the image sprite's pivot
    void Update()
    {
        SetPivot();
    }

    public void SetPivot()
    {
        //set the rectransform of the gameobject to be equal to the porportions of the sprite
        gameObject.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<Image>().sprite.rect.size * 7;

        //set the pivot of the image to equal the pivot of the sprite in order to show sprite animations properly
        var newPivot = gameObject.GetComponent<Image>().sprite.pivot / gameObject.GetComponent<Image>().sprite.rect.size;
        gameObject.GetComponent<RectTransform>().pivot = new Vector2(Mathf.Round(newPivot.x * 100f) * 0.01f, Mathf.Round(newPivot.y * 100f) * 0.01f);
        
    }
}

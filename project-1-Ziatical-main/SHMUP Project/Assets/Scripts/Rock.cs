using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    //Current Game Object
    private SpriteRenderer currentSprite;

    //Positions
    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 bottomLeft;
    public Vector3 bottomRight;

    //Location Center
    public Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        topRight = currentSprite.transform.TransformPoint(currentSprite.sprite.bounds.max);
        topLeft = currentSprite.transform.TransformPoint(new Vector3(currentSprite.sprite.bounds.max.x, currentSprite.sprite.bounds.min.y, 0));
        bottomLeft = currentSprite.transform.TransformPoint(currentSprite.sprite.bounds.min);
        bottomRight = currentSprite.transform.TransformPoint(new Vector3(currentSprite.sprite.bounds.min.x, currentSprite.sprite.bounds.max.y, 0));
        center = currentSprite.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        topRight = currentSprite.transform.TransformPoint(currentSprite.sprite.bounds.max);
        topLeft = currentSprite.transform.TransformPoint(new Vector3(currentSprite.sprite.bounds.max.x, currentSprite.sprite.bounds.min.y, 0));
        bottomLeft = currentSprite.transform.TransformPoint(currentSprite.sprite.bounds.min);
        bottomRight = currentSprite.transform.TransformPoint(new Vector3(currentSprite.sprite.bounds.min.x, currentSprite.sprite.bounds.max.y, 0));
        center = currentSprite.transform.position;
    }

    //Colliding Method
    public Rock Colliding(GameObject obj = null)
    {
        if (obj != null)
        {
            Vector2 maxXY1 = currentSprite.bounds.max;
            Vector2 minXY1 = currentSprite.bounds.min;

            SpriteRenderer objSp = obj.gameObject.GetComponent<SpriteRenderer>();
            Vector2 maxXY2 = objSp.bounds.max;
            Vector2 minXY2 = objSp.bounds.min;

            // Check for collision between this Rock and the other object
            if (((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x)) && ((maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y)))
            {
                return this;
            }
        }
        return null;
    }
}

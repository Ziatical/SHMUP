using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    public SpriteRenderer currentSprite;
    public int timer;
    public bool countdown;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>();
        timer = 5000;
        countdown = false;
        currentSprite.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown)
        {
            timer -= 1;
        }
        if (timer <= 0)
        {
            countdown = false;
        }
    }

    //Colliding Method
    public Slow Colliding(GameObject obj = null)
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

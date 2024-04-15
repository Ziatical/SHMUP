using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseBullet : MonoBehaviour
{
    private Vector3 movement;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    Mouse mouse;
    GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
    }
    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3(0.04f, 0, 0);
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movement;
        if (transform.position.x >= maxPosition.x)
        {
            Destroy(this.gameObject);
            mouse.bullets.Remove(this.gameObject);
        }
        //current bullet position and location
        SpriteRenderer bulletSprite = this.GetComponent<SpriteRenderer>();

        //sprite location min and max x and y
        Vector2 maxXY2 = bulletSprite.bounds.max;
        Vector2 minXY2 = bulletSprite.bounds.min;
        //IF IT COLLIDES WITH A CAT
        foreach (GameObject cat in gameManager.cats)
        {
            SpriteRenderer catSprite = cat.GetComponent<SpriteRenderer>();

            //Sprite location for cat
            Vector2 maxXY1 = catSprite.bounds.max;
            Vector2 minXY1 = catSprite.bounds.min;

            //removes bullet and destroys bullet if collision
            if((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x) && (maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y))
            {
                mouse.bullets.Remove(this.gameObject);
                Destroy(this.gameObject);
                gameManager.cats.Remove(cat);
                Destroy(cat);
                return;
            }
        }
        foreach (GameObject fox in gameManager.foxes)
        {
            SpriteRenderer foxSprite = fox.GetComponent<SpriteRenderer>();

            //Sprite location for cat
            Vector2 maxXY3 = foxSprite.bounds.max;
            Vector2 minXY3 = foxSprite.bounds.min;

            //removes bullet and destroys bullet if collision
            if((minXY3.x < maxXY2.x) && (maxXY3.x > minXY2.x) && (maxXY3.y > minXY2.y) && (minXY3.y < maxXY2.y))
            {
                mouse.bullets.Remove(this.gameObject);
                Destroy(this.gameObject);
                gameManager.foxes.Remove(fox);
                Destroy(fox);
                gameManager.score += 30;
                return;
            }
        }
    }
}

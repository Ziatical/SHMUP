using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairballBullet : MonoBehaviour
{
    private Vector3 movement;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    Mouse mouse;
    GameManager gameManager;
    Cat cat;
    Fox fox;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        movement = (transform.localScale.x < 0 ? new Vector3(-0.015f, 0, 0) : new Vector3(0.015f, 0, 0)); // Speed set to match current speed but direction is dynamic
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        transform.position -= movement;
        if (transform.position.x <= minPosition.x || transform.position.x >= maxPosition.x)
        {
            foreach(GameObject catObj in gameManager.cats)
            {
                foreach(GameObject foxObj in gameManager.foxes)
                {
                    if (catObj.GetComponent<Cat>().bullets.Contains(this.gameObject))
                    {
                        Destroy(this.gameObject);
                        catObj.GetComponent<Cat>().bullets.Remove(this.gameObject);
                        gameManager.score += 10;
                        return;
                    }
                    if (foxObj.GetComponent<Fox>().bullets.Contains(this.gameObject))
                    {
                        Destroy(this.gameObject);
                        foxObj.GetComponent<Fox>().bullets.Remove(this.gameObject);
                        gameManager.score += 10;
                        return;
                    }
                }
            }
        }
        //current bullet position and location
        SpriteRenderer bulletSprite = this.GetComponent<SpriteRenderer>();

        //sprite location min and max x and y
        Vector2 maxXY2 = bulletSprite.bounds.max;
        Vector2 minXY2 = bulletSprite.bounds.min;
        //IF IT COLLIDES WITH A MOUSE
        SpriteRenderer mouseSprite = mouse.gameObject.GetComponent<SpriteRenderer>();
        Vector2 maxXY1 = mouseSprite.bounds.max;
        Vector2 minXY1 = mouseSprite.bounds.min;
        //removes bullet and destroys bullet if collision
        if(((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x)) && ((maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y)))
        {
            foreach(GameObject catObj in gameManager.cats)
            {
                foreach(GameObject foxObj in gameManager.foxes)
                {

                    if (catObj.GetComponent<Cat>().bullets.Contains(this.gameObject))
                    {
                        Destroy(this.gameObject);
                        catObj.GetComponent<Cat>().bullets.Remove(this.gameObject);
                        gameManager.health -= 5;
                        return;
                    }
                    else if (foxObj.GetComponent<Fox>().bullets.Contains(this.gameObject))
                    {
                        Destroy(this.gameObject);
                        foxObj.GetComponent<Fox>().bullets.Remove(this.gameObject);
                        gameManager.health -= 5;
                        return;
                    }
                }
            }
        }
    }

    public void InitializeDirection(float directionX)
    {
        movement = new Vector3(0.015f * directionX, 0, 0); // Adjust the movement vector based on the direction
    }
}

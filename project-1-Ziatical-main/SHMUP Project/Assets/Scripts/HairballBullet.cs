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
    BossManager bossManager;
    Cat cat;
    Fox fox;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        catch
        {
            bossManager = GameObject.Find("BossManager").GetComponent<BossManager>();
        }
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        movement = (transform.localScale.x < 0 ? new Vector3(-0.1f, 0, 0) : new Vector3(0.1f, 0, 0)); // Speed set to match current speed but direction is dynamic
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        // Assuming speed should be scaled by deltaTime to ensure frame-independent movement
        transform.position -= movement * Time.deltaTime * 100;  // Adjust the multiplier to control speed
        if (transform.position.x <= minPosition.x || transform.position.x >= maxPosition.x)
        {
            if (gameManager != null)
            {
                foreach (GameObject catObj in gameManager.cats)
                {
                    foreach (GameObject foxObj in gameManager.foxes)
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
            else
            {
                Destroy(this.gameObject);
                if (bossManager != null && bossManager.bullets.Contains(this.gameObject))
                {
                    Destroy(this.gameObject);
                    bossManager.bullets.Remove(this.gameObject);
                    bossManager.score += 10;
                    return;
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
            if (gameManager != null)
            {
                foreach (GameObject catObj in gameManager.cats)
                {
                    foreach (GameObject foxObj in gameManager.foxes)
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
            else
            {
                if (bossManager != null && bossManager.bullets.Contains(this.gameObject))
                {
                    Destroy(this.gameObject);
                    bossManager.bullets.Remove(this.gameObject);
                    bossManager.health -= 5;
                    return;
                }
            }
        }
    }

    public void InitializeDirection(Vector3 direction)
    {
        movement = direction * 0.015f; // Adjust bullet speed if necessary
    }
}

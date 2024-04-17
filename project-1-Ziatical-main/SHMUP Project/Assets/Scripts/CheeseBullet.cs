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
    BossManager bossManager;

    void Awake()
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
    }
    // Start is called before the first frame update
    void Start()
    {
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
        transform.position += movement * Time.deltaTime * 100;  // Adjust the multiplier to control speed
        if (transform.position.x <= minPosition.x || transform.position.x >= maxPosition.x)
        {
            SafeDestroy();
        }
        //current bullet position and location
        SpriteRenderer bulletSprite = this.GetComponent<SpriteRenderer>();

        //sprite location min and max x and y
        Vector2 maxXY2 = bulletSprite.bounds.max;
        Vector2 minXY2 = bulletSprite.bounds.min;
        //IF IT COLLIDES WITH A CAT
        if (gameManager != null)
        {
            foreach (GameObject cat in gameManager.cats)
            {
                SpriteRenderer catSprite = cat.GetComponent<SpriteRenderer>();

                //Sprite location for cat
                Vector2 maxXY1 = catSprite.bounds.max;
                Vector2 minXY1 = catSprite.bounds.min;

                //removes bullet and destroys bullet if collision
                if ((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x) && (maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y))
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
                if ((minXY3.x < maxXY2.x) && (maxXY3.x > minXY2.x) && (maxXY3.y > minXY2.y) && (minXY3.y < maxXY2.y))
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
        else
        {
            SpriteRenderer bossCatSprite = bossManager.cat.GetComponent<SpriteRenderer>();

            //Sprite location for boss cat
            Vector2 maxXY3 = bossCatSprite.bounds.max;
            Vector2 minXY3 = bossCatSprite.bounds.min;

            // removes bullet and damages the boss cat
            //removes bullet and destroys bullet if collision
            if ((minXY3.x < maxXY2.x) && (maxXY3.x > minXY2.x) && (maxXY3.y > minXY2.y) && (minXY3.y < maxXY2.y))
            {
                mouse.bullets.Remove(this.gameObject);
                Destroy(this.gameObject);
                bossManager.bossHealth -= 30;
            }
        }
    }

    public void SetMovementDirection(float direction)
    {
        movement = new Vector3(0.015f * direction, 0, 0); // Assumes positive right, negative left
    }

    private void SafeDestroy()
    {
        if (mouse.bullets.Contains(gameObject))
        {
            mouse.bullets.Remove(gameObject);
        }
        Destroy(gameObject);
    }
}

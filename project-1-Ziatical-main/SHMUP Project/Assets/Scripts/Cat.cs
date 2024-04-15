using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private Vector3 minPosition;
    private Vector3 maxPosition;
    public Vector3 movement;
    public GameObject mouse;
    public GameManager gameManager;
    private int timeToShoot;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject hairballBullet;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouse = GameObject.Find("Mouse");
    }
    void Start()
    {
        movement = new Vector3(-0.003f, 0, 0);
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        timeToShoot = 0;

    }

    // Update is called once per frame
    void Update()
    {
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        transform.position += movement;
        if (transform.position.x <= minPosition.x)
        {
            gameManager.cats.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.score += 20;
        }
        //sprites
        SpriteRenderer mouseSprite = mouse.GetComponent<SpriteRenderer>();
        SpriteRenderer catSprite = this.GetComponent<SpriteRenderer>();

        //sprite location min and max x and y
        Vector2 maxXY1 = mouseSprite.bounds.max;
        Vector2 minXY1 = mouseSprite.bounds.min;

        Vector2 maxXY2 = catSprite.bounds.max;
        Vector2 minXY2 = catSprite.bounds.max;
        if(((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x)) && ((maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y)))
        {
            gameManager.cats.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.health -= 10;
        }
        if (timeToShoot == 1000)
        {
            bullets.Add(Instantiate(hairballBullet, new Vector3((this.gameObject.transform.position.x - .2f), this.gameObject.transform.position.y, 0f), Quaternion.identity));
            timeToShoot = -1;
        }
        timeToShoot++;
    }
}

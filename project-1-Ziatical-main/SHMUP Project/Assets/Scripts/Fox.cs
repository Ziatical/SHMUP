using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fox : MonoBehaviour
{
    private Vector3 minPosition;
    private Vector3 maxPosition;
    public Vector3 movement;
    public GameObject mouse;
    public GameManager gameManager;
    private int timeToShoot;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject hairballBullet;
    public State currentState;

    //Rock current issue
    Rock issuedRock;
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
        currentState = State.Seek;
    }

    // Update is called once per frame
    void Update()
    {
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        transform.position += movement;
        if (transform.position.x <= minPosition.x)
        {
            gameManager.foxes.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.score += 20;
        }
        //sprites
        SpriteRenderer mouseSprite = mouse.GetComponent<SpriteRenderer>();
        SpriteRenderer foxSprite = this.GetComponent<SpriteRenderer>();

        //sprite location min and max x and y
        Vector2 maxXY1 = mouseSprite.bounds.max;
        Vector2 minXY1 = mouseSprite.bounds.min;

        Vector2 maxXY2 = foxSprite.bounds.max;
        Vector2 minXY2 = foxSprite.bounds.max;
        if(((minXY1.x < maxXY2.x) && (maxXY1.x > minXY2.x)) && ((maxXY1.y > minXY2.y) && (minXY1.y < maxXY2.y)))
        {
            gameManager.foxes.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.health -= 10;
        }
        if (timeToShoot == 1000)
        {
            bullets.Add(Instantiate(hairballBullet, new Vector3((this.gameObject.transform.position.x - .2f), this.gameObject.transform.position.y, 0f), Quaternion.identity));
            timeToShoot = -1;
        }
        timeToShoot++;
        if (currentState == State.Seek)
        {

        }
        else if (currentState == State.Shoot)
        {

        }
        else if (currentState == State.Manuever)
        {
            //getting issued rock
            foreach (Rock rock in gameManager.rocks)
            {
                // Check collision with the current rock
                if (rock.Colliding(this.gameObject) != null)
                {
                    // Collision detected with a rock, update movement and exit loop
                    if (this.gameObject.transform.position.y > Screen.height / 2)
                    {
                        movement = new Vector3(0, -0.003f, 0);
                        issuedRock = rock;
                        break;
                    }
                    else if (this.gameObject.transform.position.y <= Screen.height / 2)
                    {
                        movement = new Vector3(0, 0.003f, 0);
                        issuedRock = rock;
                        break;
                    }
                }
                
            }
            if (issuedRock == null || issuedRock.Colliding(this.gameObject) == null)
            {
                movement = new Vector3(-0.003f, 0, 0);
                currentState = State.Seek;
            }
        }
    }

    public enum State
    {
        Seek,
        Shoot,
        Manuever
    }
}

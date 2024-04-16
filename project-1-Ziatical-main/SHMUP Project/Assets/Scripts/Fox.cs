using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    //A* Stuff
    public float speed = 0.003f; // Adjust speed as needed
    private List<Coordinate> path; // Store the path from cat to mouse
    private int currentWaypointIndex = 0; // Index of current waypoint in the path

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

        // Calculate positions and determine the direction to face based on the mouse's position
        Vector3 mousePosition = mouse.transform.position;

        // Flipping logic based on relative position to the mouse
        if (transform.position.x < mousePosition.x && transform.localScale.x != 1)
        {
            // Face right when mouse is to the right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x > mousePosition.x && transform.localScale.x != -1)
        {
            // Face left when mouse is to the left
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Update position and handle flipping
        if (currentState == State.Seek)
        {
            FindPathToMouse();
            MoveAlongPath();
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

        HandleFlipping();

        if (timeToShoot >= 1000)
        {
            bullets.Add(Instantiate(hairballBullet, new Vector3((this.gameObject.transform.position.x - .2f), this.gameObject.transform.position.y, 0f), Quaternion.identity));
            timeToShoot = 0;
        }
        timeToShoot++;
    }

    // Find a path from the fox's current position to the mouse's position
    private void FindPathToMouse()
    {
        Vector3 foxPosition = transform.position;
        Vector3 mousePosition = mouse.transform.position;

        Coordinate start = new Coordinate(Mathf.RoundToInt(foxPosition.x), Mathf.RoundToInt(foxPosition.y));
        Coordinate target;

        // Preventing rightward target setting
        if (foxPosition.x > mousePosition.x)
        {
            target = new Coordinate(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
        }
        else
        {
            // Set target further left if passed
            target = new Coordinate(Mathf.RoundToInt(mousePosition.x) - 5, Mathf.RoundToInt(mousePosition.y));
        }

        path = AStar.FindPath(start, target);
        currentWaypointIndex = 0;
    }


    // Move the cat along the path
    private void MoveAlongPath()
    {
        if (path != null && currentWaypointIndex < path.Count)
        {
            if (mouse == null)
            {
                Debug.LogError("Mouse reference lost, stopping movement.");
                return; // Stop updating the path if the mouse is destroyed or lost
            }

            Vector3 nextWaypoint = new Vector3(path[currentWaypointIndex].x, path[currentWaypointIndex].y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, speed);

            if (Vector3.Distance(transform.position, nextWaypoint) < 0.01f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            movement = new Vector3(-0.003f, 0, 0);
            transform.position += movement * Time.deltaTime;
        }

        if (currentWaypointIndex >= path.Count)
        {
            FindPathToMouse();
            currentWaypointIndex = 0;
        }
    }

    void HandleFlipping()
    {
        Vector3 mousePosition = mouse.transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Determine if the character should face left or right based on the mouse position
        bool shouldFaceLeft = transform.position.x > mousePosition.x;

        // Use SpriteRenderer's flipX to control the direction the sprite faces
        spriteRenderer.flipX = shouldFaceLeft;
    }

    public enum State
    {
        Seek,
        Manuever
    }
}

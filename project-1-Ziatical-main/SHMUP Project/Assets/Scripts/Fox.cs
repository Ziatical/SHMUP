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
    private float timeToShoot = 1000; // Synchronize shooting interval with Cat.cs
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject hairballBullet;
    public State currentState;

    // A* pathfinding stuff
    public float speed = 0.01f;  // Increase the speed value here
    private List<Coordinate> path;
    private int currentWaypointIndex = 0;

    // Rock collision issue
    Rock issuedRock;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouse = GameObject.Find("Mouse");
    }
    void Start()
    {
        movement = new Vector3(-0.003f, 0, 0); // Match initial movement with Cat.cs
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        timeToShoot = 1000; // Start with a full interval to first shot
        currentState = State.Seek; // Default state
    }

    // Update is called once per frame
    void Update()
    {
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));

        if (transform.position.x <= minPosition.x)
        {
            gameManager.foxes.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.score += 20;
        }

        HandleFlipping();
        HandleSpriteCollision();
        HandleStateManagement();
        HandleShooting();
    }

    void HandleStateManagement()
    {
        switch (currentState)
        {
            case State.Seek:
                FindPathToMouse();
                MoveAlongPath();
                break;
            case State.Manuever:
                ManageObstacleAvoidance();
                break;
        }
    }

    void HandleSpriteCollision()
    {
        SpriteRenderer foxSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer mouseSprite = mouse.GetComponent<SpriteRenderer>();

        if (foxSprite.bounds.Intersects(mouseSprite.bounds))
        {
            gameManager.foxes.Remove(this.gameObject);
            Destroy(this.gameObject);
            gameManager.health -= 10;
        }
    }

    void HandleShooting()
    {
        timeToShoot -= Time.deltaTime * 1000;
        if (timeToShoot <= 0)
        {
            Shoot();
            timeToShoot = 1000; // Reset the timer after shooting
        }
    }

    void Shoot()
    {
        // Determine the shooting direction based on the current facing direction
        Vector3 shootDirection = transform.localScale.x < 0 ? Vector3.left : Vector3.right;

        // Instantiate the bullet and initialize its direction
        GameObject bullet = Instantiate(hairballBullet, transform.position + shootDirection * 0.2f, Quaternion.identity);
        HairballBullet bulletComponent = bullet.GetComponent<HairballBullet>();
        bulletComponent.InitializeDirection(shootDirection); // Now correctly passing Vector3
        bullets.Add(bullet);
    }

    void ManageObstacleAvoidance()
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
                    
                }
                else if (this.gameObject.transform.position.y <= Screen.height / 2)
                {
                    movement = new Vector3(0, 0.003f, 0);
                    issuedRock = rock;
                    
                }
            }

        }
        if (issuedRock == null || issuedRock.Colliding(this.gameObject) == null)
        {
            movement = new Vector3(-0.003f, 0, 0);
            currentState = State.Seek;
        }
    }

    void HandleFlipping()
    {
        Vector3 mousePosition = mouse.transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Determine the direction opposite to the current logic
        bool shouldFaceLeft = transform.position.x <= mousePosition.x;  // Reverse the condition
        if (spriteRenderer.flipX != shouldFaceLeft)
        {
            spriteRenderer.flipX = shouldFaceLeft;
        }
    }

    // Find a path from the cat's current position to the mouse's position
    private void FindPathToMouse()
    {
        Vector3 catPosition = transform.position;
        Vector3 mousePosition = mouse.transform.position;

        Coordinate start = new Coordinate(Mathf.RoundToInt(catPosition.x), Mathf.RoundToInt(catPosition.y));
        Coordinate target;

        // Preventing rightward target setting
        if (catPosition.x > mousePosition.x)
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
            Vector3 nextWaypoint = new Vector3(path[currentWaypointIndex].x, path[currentWaypointIndex].y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, speed);

            if (Vector3.Distance(transform.position, nextWaypoint) < 0.01f)
            {
                currentWaypointIndex++;
            }

            if (currentWaypointIndex >= path.Count)
            {
                FindPathToMouse();
                currentWaypointIndex = 0;
            }
        }
    }

    public enum State
    {
        Seek,
        Manuever
    }
}

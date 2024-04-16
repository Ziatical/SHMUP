using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public int score = 0;
    public Text scoreCounter;
    public Slider healthBar;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    public int spawnTime = 2000000000;
    private int spawnCheck = 0;
    public GameObject cat;
    public List<GameObject> cats = new List<GameObject>();
    public GameObject fox;
    public List<GameObject> foxes = new List<GameObject>();
    public GameObject mouse;
    private SpriteRenderer mouseSprite;
    // List of Rocks
    public List<Rock> rocks = new List<Rock>();

    [SerializeField]
    private GameObject grass;
    [SerializeField]
    private float cameraSpeed = 0.005f;
    // Start is called before the first frame update
    void Start()
    {
        mouseSprite = mouse.GetComponent<SpriteRenderer>();
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (cats != null) {
        for (int i = 0; i < cats.Count; i++)
        {
            if (cats[i] == null) {
                Debug.LogError($"Cat at index {i} is null.");
                continue;
            }
            // Log positions and states
            Debug.Log($"Cat {i} at position {cats[i].transform.position}");
        }
    }
        minPosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        maxPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        healthBar.value = health/maxHealth;
        scoreCounter.text = $"Score: {score.ToString("00000")}";
        if (spawnCheck == spawnTime)
        {
            int num = Random.Range(0, 2);
            if (num == 0)
            {
                cats.Add(Instantiate(cat, new Vector3(maxPosition.x, Random.Range(minPosition.y, maxPosition.y), 0), Quaternion.identity));
            }
            else if (num == 1)
            {
                foxes.Add(Instantiate(fox, new Vector3(maxPosition.x, Random.Range(minPosition.y, maxPosition.y), 0), Quaternion.identity));
            }
            spawnCheck = -1;
        }
        spawnCheck++;
        if (health <= 0)
        {
            SceneManager.LoadScene(0);
            health = 100;
        }
        Vector3 grassPos = Camera.main.transform.position;
        //Vector3 grassStretch = grass.transform.localScale;
        grassPos.x += cameraSpeed;
        //grassStretch.x += 0.005f;
        Camera.main.transform.position = grassPos;
        //grass.transform.localScale = grassStretch;

        //Checking if Rocks collide with any Cats
        foreach (GameObject catG in cats)
        {
            foreach(Rock rock in rocks)
            {
                if (rock.Colliding(catG) != null)
                {
                    // Change state of cat to move around rock
                    // stop movement
                    Cat catSrc = catG.GetComponent<Cat>();
                    catSrc.currentState = Cat.State.Manuever;
                }
            }
        }

        //Checking if Rocks collide with any Foxes
        foreach (GameObject foxG in foxes)
        {
            foreach (Rock rock in rocks)
            {
                if (rock.Colliding(foxG) != null)
                {
                    // Change state of cat to move around rock
                    // stop movement
                    Fox foxSrc = foxG.GetComponent<Fox>();
                    foxSrc.currentState = Fox.State.Manuever;
                }
            }
        }

        //Checking if Rocks collide with player
        foreach (Rock rock in rocks)
        {
            Mouse mouseSrc = mouse.GetComponent<Mouse>();
            if (rock.Colliding(mouse) != null)
            {
                // stop front movement ONLY
                mouseSrc.move = false;
                if (mouse.transform.position.y < Screen.height && mouse.transform.position.y > Screen.height / 2)
                {
                    Vector3 moveM = new Vector3(0, -0.03f, 0);
                    mouse.transform.position += moveM;
                }
                else
                {
                    Vector3 moveM = new Vector3(0, 0.03f, 0);
                    mouse.transform.position += moveM;
                }
            }
            mouseSrc.move = true;
        }
    }
}

public class AStar
{
    // Find a path from start to target coordinates
    public static List<Coordinate> FindPath(Coordinate start, Coordinate target)
    {
        List<Coordinate> openList = new List<Coordinate>();
        HashSet<Coordinate> closedList = new HashSet<Coordinate>();
        Dictionary<Coordinate, Coordinate> parentMap = new Dictionary<Coordinate, Coordinate>();
        Dictionary<Coordinate, int> costMap = new Dictionary<Coordinate, int>();

        openList.Add(start);
        costMap[start] = 0;

        while (openList.Count > 0)
        {
            Coordinate current = FindLowestCostCoordinate(openList, costMap, target);
            openList.Remove(current);

            if (current.Equals(target))
            {
                return ReconstructPath(start, target, parentMap);
            }

            closedList.Add(current);

            List<Coordinate> neighbors = GetNeighbors(current);
            foreach (Coordinate neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeCost = costMap[current] + 1;
                if (!openList.Contains(neighbor) || tentativeCost < costMap[neighbor])
                {
                    costMap[neighbor] = tentativeCost;
                    parentMap[neighbor] = current;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // No path found
        return new List<Coordinate>();
    }

    // Find the coordinate with the lowest total cost in the open list
    private static Coordinate FindLowestCostCoordinate(List<Coordinate> openList, Dictionary<Coordinate, int> costMap, Coordinate target)
    {
        int minCost = int.MaxValue;
        Coordinate minCostCoordinate = null;
        foreach (Coordinate coordinate in openList)
        {
            int totalCost = costMap[coordinate] + coordinate.ManhattanDistance(target);
            if (totalCost < minCost)
            {
                minCost = totalCost;
                minCostCoordinate = coordinate;
            }
        }
        return minCostCoordinate;
    }

    // Reconstruct path from start to target coordinates using parent map
    private static List<Coordinate> ReconstructPath(Coordinate start, Coordinate target, Dictionary<Coordinate, Coordinate> parentMap)
    {
        List<Coordinate> path = new List<Coordinate>();
        Coordinate current = target;

        while (!current.Equals(start))
        {
            path.Add(current);
            if (!parentMap.ContainsKey(current))
            {
                Debug.LogError($"Missing parent for coordinate: {current.x}, {current.y}");
                break; // Break the loop if no parent is found to avoid crashing
            }
            current = parentMap[current];
        }
        path.Reverse();
        return path;
    }

    // Get neighboring coordinates of a given coordinate (up, down, left, right)
    private static List<Coordinate> GetNeighbors(Coordinate coordinate)
    {
        List<Coordinate> neighbors = new List<Coordinate>();
        neighbors.Add(new Coordinate(coordinate.x - 1, coordinate.y)); // Left
        neighbors.Add(new Coordinate(coordinate.x, coordinate.y + 1)); // Up
        neighbors.Add(new Coordinate(coordinate.x, coordinate.y - 1)); // Down
        return neighbors;
    }
}

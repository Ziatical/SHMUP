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
                    catSrc.movement = new Vector3(0,0,0);
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
                    foxSrc.movement = new Vector3(0, 0, 0);
                }
            }
        }

        //Checking if Rocks collide with player
        foreach (Rock rock in rocks)
        {
            if (rock.Colliding(mouse) != null)
            {
                // stop front movement ONLY
                Mouse mouseSrc = mouse.GetComponent<Mouse>();
                mouseSrc.velocity.x = 0;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int catAmount = 10;
    public float health = 100f;
    public float maxHealth = 100f;
    public int score = 0;
    public Text scoreCounter;
    public Slider healthBar;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    public int spawnTime = 300;
    private int spawnCheck = 0;
    public GameObject cat;
    public List<GameObject> cats = new List<GameObject>();
    public GameObject fox;
    public List<GameObject> foxes = new List<GameObject>();
    public GameObject mouse;
    private SpriteRenderer mouseSprite;
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
        healthBar.value = health/maxHealth;
        scoreCounter.text = $"Score: {score.ToString("00000")}";
        if (spawnCheck == spawnTime)
        {
            cats.Add(Instantiate(cat, new Vector3(maxPosition.x, Random.Range(minPosition.y, maxPosition.y), 0), Quaternion.identity));
            foxes.Add(Instantiate(fox, new Vector3(maxPosition.x, Random.Range(minPosition.y, maxPosition.y), 0), Quaternion.identity));
            spawnCheck = -1;
        }
        spawnCheck++;
        if (health <= 0)
        {
            SceneManager.LoadScene(0);
            health = 100;
        }
    }
}

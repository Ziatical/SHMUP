using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class BossManager : MonoBehaviour
{
    public GameObject cat;
    public GameObject bulletPrefab;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject mouse;
    public GameObject healthPotionPrefab;  // Assumed health potion prefab for interactions
    public float health = 100f;
    public float maxHealth = 100;  // Max health
    public Slider healthBar;
    private State currentState = State.singleShoot;
    private float shootTimer = 0;
    public int score;
    public Text scoreCounter;

    public float speed = 100.0f; // Speed of the boss's circular movement
    private float radius = 4.0f; // Radius of the circle along which the boss moves

    private float moveDirection = 1.0f; // Direction indicator: 1 for up, -1 for down

    private Vector3 initialPosition;

    private float stateTimer = 0f;

    //boss stuff

    public float bossHealth = 200f;
    public float maxBossHealth = 200;
    public Slider bossHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = cat.transform.position;
        InvokeRepeating("ToggleShootState", 2f, 2f);  // Change state every 7 seconds, with first change after 5 seconds
        InvokeRepeating("SpawnHealthPotion", 5f, 5f); // Spawn a health potion every 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health / maxHealth;
        bossHealthBar.value = bossHealth / maxBossHealth;
        stateTimer += Time.deltaTime;
        MoveUpDown();
        HandleStates();
        CheckHealthStatus();

        if (bossHealth <= 0)
        {
            SceneManager.LoadScene(0); // Assuming 0 is the menu or a game over scene
        }
    }

    void MoveUpDown()
    {
        if (cat.transform.position.y >= initialPosition.y + radius)  // Check if cat is at the upper boundary
        {
            moveDirection = -1;  // Move down
        }
        else if (cat.transform.position.y <= initialPosition.y - radius)  // Check if cat is at the lower boundary
        {
            moveDirection = 1;  // Move up
        }

        // Update cat's vertical position
        cat.transform.position += new Vector3(0, moveDirection * speed * Time.deltaTime, 0);
    }

    void HandleStates()
    {
        switch (currentState)
        {
            case State.singleShoot:
                SingleShoot();
                break;
            case State.multiShoot:
                MultiShoot();
                break;
            case State.flee:
                Flee();
                break;
        }
    }

    void SingleShoot()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            Shoot((mouse.transform.position - cat.transform.position).normalized);
            shootTimer = 1f;  // Reset timer for single shooting
        }
    }

    void MultiShoot()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            Vector3[] directions = {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right,
            new Vector3(1, 1, 0), new Vector3(1, -1, 0),
            new Vector3(-1, 1, 0), new Vector3(-1, -1, 0),
            new Vector3(0.7f, 0.7f, 0), new Vector3(0.7f, -0.7f, 0),
            new Vector3(-0.7f, 0.7f, 0), new Vector3(-0.7f, -0.7f, 0)
        };

            foreach (var dir in directions)
            {
                Shoot(dir.normalized);  // Ensure directions are normalized
            }
            shootTimer = 2f;  // Reset timer for the next MultiShoot
        }
    }

    void Flee()
    {
        LookForHealthPotions();
        // Check if the boss is at or beyond the right edge of the screen
        if (cat.transform.position.x >= Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - 0.5f) // 0.5f as buffer for sprite size
        {
            // If at the right edge, move up and down
            if (cat.transform.position.y >= initialPosition.y + radius)  // Check if cat is at the upper boundary
            {
                moveDirection = -1;  // Move down
            }
            else if (cat.transform.position.y <= initialPosition.y - radius)  // Check if cat is at the lower boundary
            {
                moveDirection = 1;  // Move up
            }

            // Update cat's vertical position
            cat.transform.position += new Vector3(0, moveDirection * speed * Time.deltaTime, 0);
        }
        else
        {
            // If not at the edge, continue moving to the right
            cat.transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }

    void ToggleShootState()
    {
        // Toggle between SingleShoot and MultiShoot states
        currentState = currentState == State.singleShoot ? State.multiShoot : State.singleShoot;
        shootTimer = currentState == State.singleShoot ? 1f : 2f;  // Ensure this resets correctly
    }

    void CheckHealthStatus()
    {
        if (bossHealth <= maxBossHealth * 0.25f)
        {
            currentState = State.flee;
            LookForHealthPotions();
        }
        else if (bossHealth <= maxBossHealth * 0.50f)
        {
            LookForHealthPotions(); // Ensure there's logic to handle this
        }
        else
        {
            if (cat.transform.position.x != initialPosition.x)
            {
                MoveBossToCenter();
            }
        }
    }

    void SpawnHealthPotion()
    {
        Vector3 spawnPosition = new Vector3(
        Random.Range(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x,
                     Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x),
        Random.Range(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y,
                     Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y),
        0);

        GameObject potion = Instantiate(healthPotionPrefab, spawnPosition, Quaternion.identity);
        mouse.GetComponent<Mouse>().AddHealthPotion(potion.GetComponent<Health>());  // Assuming Mouse script has a method to add potions
    }

    void LookForHealthPotions()
    {
        GameObject nearestPotion = null;
        float minDistance = float.MaxValue;

        foreach (GameObject potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
        {
            float distance = Vector3.Distance(cat.transform.position, potion.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPotion = potion;
            }
        }

        if (nearestPotion != null)
        {
            cat.transform.position = Vector3.MoveTowards(cat.transform.position, nearestPotion.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(cat.transform.position, nearestPotion.transform.position) < 0.5f) // If close enough to collect
            {
                bossHealth = Mathf.Min(maxBossHealth, bossHealth + 50); // Heal the boss
                mouse.GetComponent<Mouse>().healths.Remove(nearestPotion.GetComponent<Health>());
                Destroy(nearestPotion); // Remove the potion from the game
                MoveBossToCenter();
            }
        }
    }

    void MoveBossToCenter()
    {
        // Move towards the initial position smoothly
        cat.transform.position = Vector3.MoveTowards(cat.transform.position, initialPosition, speed * Time.deltaTime);
        if (Vector3.Distance(cat.transform.position, initialPosition) < 0.1f) // Close enough to center
        {
            currentState = State.singleShoot; // Reset to normal behavior
            InvokeRepeating("ToggleShootState", 2f, 2f); // Start toggling states again if needed
            currentState = State.singleShoot; // Ensure state is reset to singleShoot to resume normal operation
        }
    }

    void Shoot(Vector3 direction)
    {
        if (bullets.Count < 100) // Ensure not to exceed bullet limit
        {
            GameObject bullet = Instantiate(bulletPrefab, cat.transform.position, Quaternion.identity);
            HairballBullet bulletComponent = bullet.GetComponent<HairballBullet>();

            // Check if currently in the MultiShoot state to scale the bullet
            if (currentState == State.multiShoot)
            {
                bullet.transform.localScale = new Vector3(3, 3, 3); // Make the bullet three times larger
            }
            else
            {
                bullet.transform.localScale = new Vector3(1, 1, 1); // Normal size for other states
            }

            bulletComponent.InitializeDirection(direction);
            bullets.Add(bullet);

            // Debugging to visualize the direction
            Debug.DrawRay(cat.transform.position, direction * 10, Color.red, 2f);
        }
    }
}

public enum State
{
    singleShoot,
    multiShoot,
    flee
}

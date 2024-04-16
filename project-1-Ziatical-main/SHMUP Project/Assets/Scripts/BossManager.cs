using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    public GameObject cat;
    public GameObject bulletPrefab;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject mouse;
    public GameObject healthPotionPrefab;  // Assumed health potion prefab for interactions
    public float health = 100f;
    private float maxHealth = 100;  // Max health
    public Slider healthBar;
    private State currentState = State.singleShoot;
    private float shootTimer = 0;
    private float stateTimer = 0;
    public int score;
    public Text scoreCounter;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ChangeState", 5f, 7f);  // Change state every 7 seconds, with first change after 5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health / maxHealth;
        HandleStates();
        CheckHealthStatus();
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
        if (shootTimer <= 0)
        {
            Vector3 direction = (mouse.transform.position - cat.transform.position).normalized;
            Shoot(direction);
            shootTimer = 1.0f;  // Shoot every second
        }
        shootTimer -= Time.deltaTime;
    }

    void MultiShoot()
    {
        if (shootTimer <= 0)
        {
            Vector3[] directions = new Vector3[]
            {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right,
            new Vector3(1, 1, 0), new Vector3(1, -1, 0),
            new Vector3(-1, 1, 0), new Vector3(-1, -1, 0)
            };

            foreach (var dir in directions)
            {
                Shoot(dir.normalized);
            }

            shootTimer = 2.0f;  // MultiShoot for 2 seconds
        }
        shootTimer -= Time.deltaTime;
    }

    void Flee()
    {
        // Implement fleeing logic
        Vector3 dir = cat.transform.position - mouse.transform.position;
        Vector3 newPos = cat.transform.position + dir.normalized * Time.deltaTime * 5;  // Example speed
        cat.transform.position = new Vector3(
            Mathf.Clamp(newPos.x, 0, Screen.width),
            Mathf.Clamp(newPos.y, 0, Screen.height),
            0);
    }

    void ChangeState()
    {
        if (currentState == State.singleShoot)
            currentState = State.multiShoot;
        else
            currentState = State.singleShoot;
    }

    void CheckHealthStatus()
    {
        if (health <= maxHealth * 0.25f)
            currentState = State.flee;
        else if (health <= maxHealth * 0.50f)
            LookForHealthPotions();
    }

    void LookForHealthPotions()
    {
        // Logic to check for nearby health potions and move towards them if found
    }

    void Shoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, cat.transform.position, Quaternion.identity);
        HairballBullet bulletComponent = bullet.GetComponent<HairballBullet>();

        // Assuming the bullet prefab's initial localScale.x is set correctly in the prefab for direction
        float directionX = direction.x >= 0 ? 1 : -1;
        bulletComponent.InitializeDirection(directionX);

        bullets.Add(bullet);
    }
}

public enum State
{
    singleShoot,
    multiShoot,
    flee
}

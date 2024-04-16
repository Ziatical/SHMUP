using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mouse : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletCheese;
    public Vector2 direction = Vector2.right;
    public Vector2 velocity = Vector2.zero;
    private Vector2 movementInput;
    public List<GameObject> bullets = new List<GameObject>();
    public bool move = true;
    public GameManager gameManager;

    //power up lists
    public List<Health> healths = new List<Health>();
    private Health currentHealth;
    public List<MoreBullets> more = new List<MoreBullets>();
    private MoreBullets currentMoreBullets;
    public List<Slow> slows = new List<Slow>();
    private Slow currentSlow;
    public List<Faster> faster = new List<Faster>();
    private Faster currentFaster;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // bullets
        if (more[0] != null)
        {
            foreach (MoreBullets bullet in more)
            {
                if (bullet.Colliding(this.gameObject) != null)
                {
                    bullet.currentSprite.enabled = false;
                    currentMoreBullets = bullet;
                    currentMoreBullets.countdown = true;
                }
            }
        }

        //regen
        if (healths[0] != null)
        {
            foreach (Health hea in healths)
            {
                if (hea.Colliding(this.gameObject) != null)
                {
                    hea.currentSprite.enabled = false;
                    currentHealth = hea;
                    currentHealth.countdown = true;
                }
            }
        }
        if (currentHealth != null && currentHealth.countdown && gameManager.health < gameManager.maxHealth)
        {
            gameManager.health += 0.5f;
        }

        if (slows[0] != null)
        {
            // slowed enemies
            foreach (Slow slow in slows)
            {
                if (slow.Colliding(this.gameObject) != null)
                {
                    slow.currentSprite.enabled = false;
                    currentSlow = slow;
                    currentSlow.countdown = true;
                }
            }
        }
        if (currentSlow != null && currentSlow.countdown)
        {
            foreach (GameObject cat in gameManager.cats)
            {
                Cat catS = cat.GetComponent<Cat>();
                catS.speed = 0.001f;
            }
            foreach(GameObject fox in gameManager.foxes)
            {
                Fox foxS = fox.GetComponent<Fox>();
                foxS.speed = 0.001f;
            }
        }
        else
        {
            foreach (GameObject cat in gameManager.cats)
            {
                Cat catS = cat.GetComponent<Cat>();
                catS.speed = 0.003f;
            }
            foreach (GameObject fox in gameManager.foxes)
            {
                Fox foxS = fox.GetComponent<Fox>();
                foxS.speed = 0.003f;
            }
        }
        ScreenStop();
        if (move)
        {
            direction = movementInput;
            velocity = direction * speed * Time.deltaTime;
            if (faster[0] != null)
            {
                foreach (Faster fast in faster)
                {
                    if (fast.Colliding(this.gameObject) != null)
                    {
                        fast.currentSprite.enabled = false;
                        currentFaster = fast;
                        currentFaster.countdown = true;
                    }
                }
            }
            if (currentFaster != null && currentFaster.countdown)
            {
                velocity = velocity * 2;
            }
            transform.position += (Vector3)velocity;

            // Flip mouse based on movement direction
            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //onMove
    public void OnMove(InputAction.CallbackContext moveContext)
    {
        movementInput = moveContext.ReadValue<Vector2>();
    }
    //onSHoot
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentMoreBullets != null && currentMoreBullets.countdown)
            {
                GameObject bullet2 = Instantiate(bulletCheese, transform.position + new Vector3(direction.x * 0.2f, 0, 0), Quaternion.identity);
                bullet2.transform.localScale = new Vector3(transform.localScale.x, 1, 1); // Ensure bullet faces the right direction
                bullet2.GetComponent<CheeseBullet>().SetMovementDirection(transform.localScale.x); // Add this method to CheeseBullet
                bullets.Add(bullet2);
            }
            GameObject bullet = Instantiate(bulletCheese, transform.position + new Vector3(direction.x * 0.2f, 0, 0), Quaternion.identity);
            bullet.transform.localScale = new Vector3(transform.localScale.x, 1, 1); // Ensure bullet faces the right direction
            bullet.GetComponent<CheeseBullet>().SetMovementDirection(transform.localScale.x); // Add this method to CheeseBullet
            bullets.Add(bullet);
        }
    }

    //stop at edge of screen
    void ScreenStop()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(this.transform.position);
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);
        this.transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }
}

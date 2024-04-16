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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ScreenStop();
        if (move)
        {
            direction = movementInput;
            velocity = direction * speed * Time.deltaTime;
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

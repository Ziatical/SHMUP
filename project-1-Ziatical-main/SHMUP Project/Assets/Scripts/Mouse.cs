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
        if (!gameObject.activeInHierarchy) // or if (!gameObject.scene.IsValid())
        {
            return;
        }
        if (context.started)
        {
            return;
        }
        if (context.canceled)
        {
            return;
        }
        if(context.performed)
        {
            bullets.Add(Instantiate(bulletCheese, new Vector3((this.gameObject.transform.position.x + .2f), this.gameObject.transform.position.y, 0f), Quaternion.identity));
        }
        return;
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

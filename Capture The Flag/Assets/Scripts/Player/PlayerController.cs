using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Controls")]
    [SerializeField] float playerMoveSpeed = 2f;
    float speedX, speedY;
    private Vector2 moveDirection;
    private Vector2 mousePosition;
    [Space(5)]

    [Header("Other Settings")]
    [SerializeField] float dashSpeed = 5f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] float fireCooldown = 0.4f;
    public bool isDashing = false;
    public bool canDash = true;
    public bool canFire = true;
    [Space(5)]

    [Header("Important Variables")]
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject enemyAI;
    [SerializeField] GameObject playerFlag;
    [SerializeField] GameObject aiFlag;
    public Vector2 playerSpawn;
    [Space(5)]

    [Header("Debugging")]
    [SerializeField] public bool hasFlag = false;
    public bool canPlay = true;

    void Start()
    {
        playerSpawn = this.gameObject.transform.position;
        playerRB = GetComponent<Rigidbody2D>(); //Sets Rigid Body
    }

    private void Update()
    {
        if (canPlay)
        {
            if (isDashing) //Prevents code below being called while dashing
            {
                return;
            }

            processMovement(); //Checks inputs to calculate movement

            if (Input.GetMouseButton(0) && (canFire == true)) //Player clicks to fire
            {
                canFire = false;
                weapon.Fire();
                StartCoroutine(WaitToFire());
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) //Player dashes with shift
            {
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) //Prevents code below being called while dashing
        {
            return;
        }

        Move();
    }

    private void processMovement()
    {
        speedX = Input.GetAxisRaw("Horizontal"); //Horizontal Movement vector info
        speedY = Input.GetAxisRaw("Vertical"); //Vertical Movement vector info

        moveDirection = new Vector2(speedX, speedY).normalized; //Moves the player

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Calculates mouse position on the screen
    }

    private void Move()
    {
        playerRB.velocity = new Vector2(moveDirection.x * playerMoveSpeed, moveDirection.y * playerMoveSpeed);

        Vector2 aimDirection = mousePosition - playerRB.position; //Get mouse position in relation to the player
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f; //Calculate which direction the player should face
        playerRB.rotation = aimAngle; //Rotates the player
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        playerRB.velocity = new Vector2(moveDirection.x * dashSpeed, moveDirection.y * dashSpeed);
        //Debug.Log("Player Dashed");
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        //Debug.Log("Dash is off cooldown");
    }

    private IEnumerator WaitToFire()
    {
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyFlag") && !hasFlag) //If collecting the enemies flag, enable that the player is holding the flag
        {
            //Debug.Log("Player grabbed the flag");
            aiFlag.transform.SetParent(transform, true);
            collision.GetComponent<FlagManager>().flagAtBase = false;
            hasFlag = true;
        }
        if (collision.gameObject.CompareTag("FriendlyFlag") && !playerFlag.GetComponent<FlagManager>().flagAtBase && !enemyAI.GetComponent<AIMovement>().aiHasFlag)
        {
            playerFlag.GetComponent<FlagManager>().flagAtBase = true;
        }
        if (collision.gameObject.CompareTag("FriendlyBuilding") && hasFlag) //If colliding with friendly base, drop flag and increase score
        {
            //Debug.Log("Player deposited flag and earned 1 point");
            aiFlag.transform.SetParent(null, true);
            gameManager.GetComponent<GameManager>().playerWins();
            hasFlag = false;
        }
        //if (collision.gameObject.CompareTag("AI") && collision.GetComponent<AIMovement>().aiHasFlag && isDashing)
        //{
        //    Debug.Log("Player bumped AI and made them drop the flag");
        //    enemyAI.GetComponent<AIMovement>().aiHasFlag = false;
        //    playerFlag.transform.SetParent(null, true);
        //    playerFlag.GetComponent<FlagManager>().flagAtBase = true;
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AIBullet"))
        {
            hasFlag = false;
            aiFlag.transform.SetParent(null, true);
            this.gameObject.transform.position = playerSpawn;
            canFire = true;
            canDash = true;
            gameManager.GetComponent<GameManager>().enemyGotKill();
            gameManager.GetComponent<GameManager>().hasDied(gameObject);
            //Debug.Log("Player was hit by a bullet and dropped the flag");
        }
    }
}

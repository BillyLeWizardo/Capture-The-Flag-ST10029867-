using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class AIMovement : MonoBehaviour
{
    [Header("NavMeshAgentSettings")]
    [SerializeField] NavMeshAgent agent;
    private Vector2 target;
    private Vector2 target2;

    [Header("Important AI")]
    [SerializeField] Rigidbody2D aiRB;
    [SerializeField] Weapon aiWeapon;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerFlag;
    [SerializeField] GameObject aiFlag;
    [SerializeField] GameObject aiBase;
    public Vector3 aiSpawn;
    [Space(5)]

    [Header("Debugging")]
    [SerializeField] public bool aiHasFlag = false;
    [SerializeField] public bool aiCanFire = true;
    [SerializeField] private float fireCooldown = 0.4f;
    public bool canPlay = true;

    //State Machine Things
    public enum aiState
    {
        TargetingFlag,
        RetrievingFlag,
        ChasingPlayer,
        TargetingPlayer,
        RunningFromPlayer,
        RetreivingOwnFlag,
        FightingForOwnFlag,
        DriveBy
    }
    private aiState currentState;

    private void Start()
    {
        aiSpawn = gameObject.transform.position;
        //Debug.Log("AI spawn is set to (" + aiSpawn.x + ", " + aiSpawn.y + ")");
    }

    private void Awake()
    {
        aiRB = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    void Update()
    {
        if (canPlay)
        {
            // Handle input or other conditions to determine state transitions
            if (!aiHasFlag && !player.GetComponent<PlayerController>().hasFlag)
            {
                SetState(aiState.TargetingFlag);
                if ((Vector2.Distance(gameObject.transform.position, player.transform.position) <= 8) && !aiHasFlag && !player.GetComponent<PlayerController>().hasFlag)
                {
                    SetState(aiState.DriveBy);
                }
            }
            if (player.GetComponent<PlayerController>().hasFlag && !aiHasFlag)
            {
                if ((Vector2.Distance(gameObject.transform.position, playerFlag.transform.position)) < (Vector2.Distance(gameObject.transform.position, player.transform.position)))
                {
                    SetState(aiState.TargetingFlag);
                }
                else
                {
                    SetState(aiState.ChasingPlayer);
                }
                if (Vector2.Distance(gameObject.transform.position, player.transform.position) <= 8)
                {
                    SetState(aiState.TargetingPlayer);
                }
            }
            if (!aiFlag.GetComponent<FlagManager>().flagAtBase && !player.GetComponent<PlayerController>().hasFlag)
            {
                SetState(aiState.RetreivingOwnFlag);
                if ((Vector2.Distance(gameObject.transform.position, player.transform.position) <= 8) && !aiFlag.GetComponent<FlagManager>().flagAtBase)
                {
                    SetState(aiState.FightingForOwnFlag);
                }
            }
            if (aiHasFlag)
            {
                SetState(aiState.RetrievingFlag);
                if ((Vector2.Distance(gameObject.transform.position, player.transform.position) <= 8) && aiHasFlag)
                {
                    SetState(aiState.RunningFromPlayer);
                }
            }


            // Perform state-specific behavior
            PerformStateBehavior();

            //Always face the target
            Vector2 aimDirection = target - aiRB.position;
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
            aiRB.rotation = aimAngle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FriendlyFlag") && !aiHasFlag) //If collecting the enemies flag, enable that the player is holding the flag
        {
            //Debug.Log("Enemy grabbed the flag");
            playerFlag.transform.SetParent(transform, true);
            collision.GetComponent<FlagManager>().flagAtBase = false;
            aiHasFlag = true;
        }
        if (collision.gameObject.CompareTag("EnemyFlag") && !aiFlag.GetComponent<FlagManager>().flagAtBase && !player.GetComponent<PlayerController>().hasFlag)
        {
            aiFlag.GetComponent<FlagManager>().flagAtBase = true;
        }
        if (collision.gameObject.CompareTag("EnemyBuilding") && aiHasFlag) //If colliding with friendly base, drop flag and increase score
        {
            //Debug.Log("Enemy deposited flag and earned 1 point");
            playerFlag.transform.SetParent(null, true);
            gameManager.GetComponent<GameManager>().aiWins();
            aiHasFlag = false;
        }
        //if (collision.gameObject.CompareTag("Player") && collision.GetComponent<PlayerController>().hasFlag && !player.GetComponent<PlayerController>().isDashing)
        //{
        //    Debug.Log("AI Bumped Player and made them drop the flag");
        //    player.GetComponent<PlayerController>().hasFlag = false;
        //    aiFlag.transform.SetParent(null, true);
        //    aiFlag.GetComponent<FlagManager>().flagAtBase = true;
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            aiHasFlag = false;
            playerFlag.transform.SetParent(null, true);
            gameObject.transform.position = aiSpawn;
            aiCanFire = true;
            gameManager.GetComponent<GameManager>().playerGotKill();
            gameManager.GetComponent<GameManager>().hasDied(gameObject);
            //Debug.Log("ai was hit by a bullet and dropped the flag");
        }
    }

    // Method to set the current state
    private void SetState(aiState newState)
    {
        currentState = newState;
        // Perform any additional actions when entering a new state
        EnterState();
    }

    // Method to handle state-specific behavior
    private void PerformStateBehavior()
    {
        // Implement behavior specific to each state
        switch (currentState)
        {
            case aiState.TargetingFlag:
                // Code for when the ai targets the flag
                target = playerFlag.transform.position;
                SetAgentPosition();
                break;
            case aiState.RetrievingFlag:
                // Code for when the ai has the player's flag
                target = aiBase.transform.position;
                SetAgentPosition();
                break;
            case aiState.ChasingPlayer:
                //Code for when the ai must chase the player
                target = player.transform.position;
                SetAgentPosition();
                break;
            case aiState.TargetingPlayer:
                // Code for when the ai attacks the player
                target = player.transform.position;
                SetAgentPosition();
                if (aiCanFire)
                {
                    aiCanFire = false;
                    aiWeapon.Fire();
                    StartCoroutine(WaitToFire());
                }
                break;
            case aiState.RunningFromPlayer:
                //Code for when the ai has the player's flag and the player is close enough to be shot at
                target = player.transform.position;
                target2 = aiBase.transform.position;
                SetAgentPosition2();
                if (aiCanFire)
                {
                    aiCanFire = false;
                    aiWeapon.Fire();
                    StartCoroutine(WaitToFire());
                }
                break;
            case aiState.RetreivingOwnFlag:
                //Code for if the ai must fetch it's own flag to return it to their base
                target = aiFlag.transform.position;
                SetAgentPosition();
                break;
            case aiState.FightingForOwnFlag:
                //Code for if the ai must fight the player for their own flag
                target = player.transform.position;
                target2 = aiFlag.transform.position;
                SetAgentPosition2();
                if (aiCanFire)
                {
                    aiCanFire = false;
                    aiWeapon.Fire();
                    StartCoroutine(WaitToFire());
                }
                break;
            case aiState.DriveBy:
                target = player.transform.position;
                target2 = playerFlag.transform.position;
                SetAgentPosition2();
                if (aiCanFire)
                {
                    aiCanFire = false;
                    aiWeapon.Fire();
                    StartCoroutine(WaitToFire());
                }
                break;
        }
    }

    // Method to handle actions when entering a new state
    private void EnterState()
    {
        // Implement any actions needed when entering a new state
    }

    //void SetTargetPosition()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    }
    //}

    void SetAgentPosition()
    {
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }
    void SetAgentPosition2()
    {
        agent.SetDestination(new Vector3(target2.x, target2.y, transform.position.z));
    }


    private IEnumerator WaitToFire()
    {
        yield return new WaitForSeconds(fireCooldown);
        aiCanFire = true;
    }

}

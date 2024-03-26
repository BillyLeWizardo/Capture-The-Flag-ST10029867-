using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class GameManager : MonoBehaviour
{
    [Header("Scores")]
    [SerializeField] public int playerScore = 0;
    [SerializeField] public int AIScore = 0;

    [SerializeField] GameObject playerFlag;
    [SerializeField] GameObject player;

    [SerializeField] GameObject AIFlag;
    [SerializeField] GameObject AI;

    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI PlayerScoreText;
    [SerializeField] private TextMeshProUGUI AIScoreText;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private TextMeshProUGUI WinText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] float respawnTimer = 2f;
    [SerializeField] float announcementTimer = 2f;

    private void Start()
    {
        winPanel.SetActive(false);
        WinText.text = "";
        StartCoroutine(GameStart());
    }

    private void Update()
    {
        //Updating Score Text
        PlayerScoreText.text = "PLAYER POINTS: " + playerScore;
        AIScoreText.text = "ENEMY POINTS: " + AIScore;

        //Winning Conditions
        if (playerScore == 5)
        {
            GameOver(true);
        }
        else if (AIScore == 5)
        {
            GameOver(false);
        }
    }

    private void GameOver(bool winner) //Triggers win text and freezes the game
    {
        if (winner)
        {
            winPanel.SetActive(true);
            WinText.text = "THE PLAYER WINS!";
            Time.timeScale = 0;
        }
        else
        {
            winPanel.SetActive(true);
            WinText.text = "THE ENEMY WINS!";
            Time.timeScale = 0;
        }
    }

    public void playerWins()
    {
        PlayerScoreAnnounce();
        playerScore++;
        
        hasDied(player);
        player.transform.position = player.GetComponent<PlayerController>().playerSpawn;
        player.GetComponent<PlayerController>().canFire = true;
        player.GetComponent<PlayerController>().canDash = true;

        hasDied(AI);
        AI.transform.position = AI.GetComponent<AIMovement>().aiSpawn;
        AI.GetComponent<AIMovement>().aiCanFire = true;
        
        playerFlag.GetComponent<FlagManager>().flagAtBase = true;
        playerFlag.transform.SetParent(null, true);

        AIFlag.GetComponent<FlagManager>().flagAtBase = true;
        AIFlag.transform.SetParent(null, true);

        player.GetComponent<PlayerController>().hasFlag = false;
        
        AI.GetComponent<AIMovement>().aiHasFlag = false;
    }

    public void aiWins()
    {
        EnemyScoreAnnounce();
        AIScore++;
        
        hasDied(player);
        player.transform.position = player.GetComponent<PlayerController>().playerSpawn;
        player.GetComponent<PlayerController>().canFire = true;
        player.GetComponent<PlayerController>().canDash = true;
        
        hasDied(AI);
        AI.transform.position = AI.GetComponent<AIMovement>().aiSpawn;
        AI.GetComponent<AIMovement>().aiCanFire = true;
        
        playerFlag.GetComponent<FlagManager>().flagAtBase = true;
        playerFlag.transform.SetParent(null, true);
        
        AIFlag.GetComponent<FlagManager>().flagAtBase = true;
        AIFlag.transform.SetParent(null, true);
        
        player.GetComponent<PlayerController>().hasFlag = false;
        
        AI.GetComponent<AIMovement>().aiHasFlag = false;
    }

    public void hasDied(GameObject person)
    {
        StartCoroutine(respawnCooldown(person));
    }

    private IEnumerator respawnCooldown(GameObject person)
    {
        person.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnTimer);
        person.gameObject.SetActive(true);
    }

    public IEnumerator PlayerScoreAnnounce()
    {
        announcementText.text = "THE PLAYER SCORED A POINT!";
        yield return new WaitForSeconds(announcementTimer);
        announcementText.text = "";
    }

    public IEnumerator EnemyScoreAnnounce()
    {
        announcementText.text = "THE ENEMY SCORED A POINT!";
        yield return new WaitForSeconds(announcementTimer);
        announcementText.text = "";
    }

    public IEnumerator PlayerKillAnnounce()
    {
        announcementText.text = "THE PLAYER GOT A KILL!";
        yield return new WaitForSeconds(announcementTimer);
        announcementText.text = "";
    }

    public IEnumerator EnemyKillAnnounce()
    {
        announcementText.text = "THE ENEMY GOT A KILL!";
        yield return new WaitForSeconds(announcementTimer);
        announcementText.text = "";
    }

    private IEnumerator GameStart()
    {
        player.GetComponent<PlayerController>().canPlay = false;
        AI.GetComponent<AIMovement>().canPlay = false;
        announcementText.text = "3";
        yield return new WaitForSeconds(1);
        announcementText.text = "2";
        yield return new WaitForSeconds(1);
        announcementText.text = "1";
        yield return new WaitForSeconds(1);
        announcementText.text = "BEGIN!";
        player.GetComponent<PlayerController>().canPlay = true;
        AI.GetComponent<AIMovement>().canPlay = true;
        yield return new WaitForSeconds(1);
        announcementText.text = "";
    }

    public void playerGotKill()
    {
        StartCoroutine(PlayerKillAnnounce());
    }

    public void enemyGotKill()
    {
        StartCoroutine(EnemyKillAnnounce());
    }

    public void Rematch()
    {
        SceneManager.LoadScene("TestMap");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

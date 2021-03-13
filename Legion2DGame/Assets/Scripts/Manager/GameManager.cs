using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Main player")]
    public GameObject player;

    [Header("Spawn points")]
    public GameObject playerSpawnPoint;
    public GameObject lvlGeneratorPortal;
    public int playerLives = 2;
    [SerializeField]
    private float respawnTime;

    [Header("Player HUD")]
    public Text GameOverHUD;
    public Slider HealthBarHUD;
    public Text LivesCountHUD;
    public Text KillCountHUD;
    public Text PlayerArrowCountHUD;
    public Text PlayerDashCounterHUD;
    public GameObject PlayerAbilityOrbHUD;
    private float dashCooldownTime;

    [Header("Random Level Generator")]
    public LevelGenerator LevelGenerator;

    private CinemachineVirtualCamera CVC;

    private bool startNewGame;
    private float startNewGameTimeStart;

    private float respawnTimeStart;
    private bool respawn;
    private bool respawning;

    private int enemyKillCount;

    private void Start()
    {
        CVC = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        startNewGame = false;
        respawn = false;
        respawning = false;
        enemyKillCount = 0;
        GameOverHUD.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!player.GetComponent<Player>().alive && !respawning)
        {
            Respawn();
        }
        CheckRespawn();

        CheckPlayerHealthBar();
        CheckPlayerLives();
        CheckPlayerKills();
        CheckPlayerArrowCount();
        CheckPlayerDashTimer();
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
        respawning = true;
        playerLives--;
    }

    public void EnemyKilled()
    {
        enemyKillCount++;
    }

    private void GameOver()
    {
        GameOverHUD.gameObject.SetActive(true);
    }

    private void StartNewGame()
    {
        startNewGame = false;
        playerLives = 2;
        enemyKillCount = 0;
        GameOverHUD.gameObject.SetActive(false);

        Respawn();
    }

    private void CheckRespawn()
    {
        if (playerLives <= 0 && !startNewGame)
        {
            GameOver();
            startNewGame = true;
            startNewGameTimeStart = Time.time;
        }
        else if (Time.time >= startNewGameTimeStart + respawnTime && startNewGame)
        {
            StartNewGame();
        }
        else if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            player.transform.position = playerSpawnPoint.transform.position;
            CVC.m_Follow = player.transform;
            player.GetComponent<Player>().Heal(player.GetComponent<Player>().maxHealth);
            player.SetActive(true);
            player.GetComponent<Player>().Resurrect();
            respawn = false;
            respawning = false;
        }
    }

    private void CheckPlayerHealthBar()
    {
        HealthBarHUD.value = player.GetComponent<Player>().currentHealth / 100;
    }

    private void CheckPlayerLives()
    {
        LivesCountHUD.text = "Lives: " + playerLives;
    }

    private void CheckPlayerKills()
    {
        KillCountHUD.text = "Kills: " + enemyKillCount;
    }

    private void CheckPlayerArrowCount()
    {
        PlayerArrowCountHUD.text = "Arrows: " + player.GetComponent<Player>().arrowCount;
    }

    private void CheckPlayerDashTimer()
    {
        // ToDo: Make this check better and reusable
        if (player.GetComponent<Player>().StateMachine.CurrentState.ToString() == "PlayerDashState")
        {
            dashCooldownTime = player.GetComponent<Player>().dashCooldown;
        }

        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.deltaTime;
            PlayerDashCounterHUD.text =  dashCooldownTime.ToString();
        }
        else
        {
            PlayerDashCounterHUD.text = "Current ability: " + player.GetComponent<Player>().currentAbility;
        }

        PlayerAbilityOrbHUD.GetComponent<HUDOrbScript>().currentValue = player.GetComponent<Player>().currentAbility;
    }
}
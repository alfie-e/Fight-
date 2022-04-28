using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public List<PlayerInput> playerList = new List<PlayerInput>();

    public GameObject[] SpawnPoints;

    [SerializeField]
    public GameObject pauseMenu;

    public static GameManager instance = null;

    [SerializeField] InputAction joinAction;
    [SerializeField] InputAction leaveAction;

    //EVENTS
    public event System.Action<PlayerInput> PlayerJoinedGame;

    public event System.Action<PlayerInput> PlayerLeftGame;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        joinAction.Enable();
        joinAction.performed += context => JoinAction(context);

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player Joined");
        playerList.Add(playerInput);

        if (PlayerJoinedGame! != null)
        {
            PlayerJoinedGame(playerInput);
        }
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("Player Left");
    }

    public void PauseGame()
    {
        TimePause();
        pauseMenu.SetActive(true);
    }

    public void UnpauseGame()
    {
        TimePlay();
        pauseMenu.SetActive(false);
    }

    void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    void LeaveAction(InputAction.CallbackContext context)
    {

    }

    public void TimePause()
    {
        Time.timeScale = 0;
    }

    public void TimePlay()
    {
        Time.timeScale = 1;
    }
}
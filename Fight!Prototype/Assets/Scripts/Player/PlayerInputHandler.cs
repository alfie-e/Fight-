using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput input;
    [SerializeField]
    Material[] playerColours;


    public GameObject PlayerPrefab;
    PlayerController PlayerController;
    InputDevice inputDevice;
    GameObject playerManager;

    public int playerIndex { get; }

    private void Awake()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager");

        input = GetComponent<PlayerInput>();
        Debug.Log($"Player {input.playerIndex} connected");

        //Create player prefab
        if (PlayerPrefab != null)
        {
            PlayerController = GameObject.Instantiate(PlayerPrefab, GameManager.instance.SpawnPoints[input.playerIndex].transform.position, transform.rotation).GetComponent<PlayerController>();
            transform.parent = (PlayerController.transform);
            transform.position = PlayerPrefab.transform.position;
        }

        transform.Translate(GameManager.instance.SpawnPoints[0].transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.device = input.currentControlScheme;

        PlayerController.playerIndex = input.playerIndex;

        SkinnedMeshRenderer mesh = transform.root.transform.GetComponentInChildren<SkinnedMeshRenderer>();

        //Choose Player Colour Based On Player Index
        switch (input.playerIndex)
        {

            case 0:
                mesh.material = playerColours[0];
                break;
            case 1:
                mesh.material = playerColours[1];
                break;
            case 2:
                mesh.material = playerColours[2];
                break;
            case 3:
                mesh.material = playerColours[3];
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerController.OnMove(context);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        PlayerController.OnAim(context);
    }

    public void OnRotateWithMouse(InputAction.CallbackContext context)
    {
        PlayerController.OnRotateWithMouse(context);
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        PlayerController.OnPickup(context);
    }

    public void OnAttack1(InputAction.CallbackContext context)
    {
        PlayerController.OnAttack1(context);
    }

    public void Pause(InputAction.CallbackContext context)
    {
        Debug.Log("pause");
        playerManager.GetComponent<GameManager>().PauseGame();
    }
}

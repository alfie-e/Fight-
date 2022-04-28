using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCounter : MonoBehaviour
{
    [SerializeField]
    GameObject[] playerScore;

    int Player1score;
    int Player2score;
    int Player3score;
    int Player4score;

    // Start is called before the first frame update
    public void addDeath(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                Player1score += 1;
                playerScore[0].GetComponent<Text>().text = Player1score.ToString();
                break;
            case 1:
                Player2score += 1;
                playerScore[1].GetComponent<Text>().text = Player2score.ToString();
                break;
            case 2:
                Player3score += 1;
                playerScore[2].GetComponent<Text>().text = Player3score.ToString();
                break;
            case 3:
                Player4score += 1;
                playerScore[3].GetComponent<Text>().text = Player4score.ToString();
                break;

        }
    }
}

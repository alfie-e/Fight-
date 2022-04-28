using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidDeathBox : MonoBehaviour
{
    [SerializeField]
    BoxCollider deathBox;

    GameObject[] spawnPoint;

    [SerializeField]
    float DragMultiplier;

    //When the player enters the collider drag is added to the player, the dying animation plays

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {
            Rigidbody rigidbody = other.transform.GetComponent<Rigidbody>();
            Animator animator = other.transform.GetComponent<Animator>();

            other.transform.GetComponent<PlayerController>().movementDisabled = true;
            animator.CrossFade("DieInLiquid", 0.5f);
            rigidbody.drag = rigidbody.drag*DragMultiplier;

        }
    }
    
    //when they exit they die so that the player isnt seen dying, drag is taken off and the animation returns to normal

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigidbody = other.transform.GetComponent<Rigidbody>();

        if (other.transform.gameObject.tag == "Player")
        {
            other.transform.GetComponent<PlayerController>().Die();
            other.transform.GetComponent<PlayerController>().movementDisabled = false;
            other.transform.GetComponent<Animator>().SetTrigger("ReturnToIdle");
            rigidbody.drag = rigidbody.drag / DragMultiplier;
        }
    }
}

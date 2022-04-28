using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    float knockbackStrength;
    [SerializeField]
    float Damage = 10;

    public BoxCollider weaponCollider;
    public GameObject WeaponGrip;

    public bool RegisterHit = false;

    public float CooldownTime = 1;
    PlayerController player;

    [SerializeField]
    string hitAudio;

    private void Start()
    {
        WeaponGrip = transform.parent.gameObject;
        player = transform.root.GetComponent<PlayerController>();
    }

    //When a player enters the collider the direction of the impact is calculated by subtracting the opponents position from the players position, that, the damage and the knockback strength
    //are transfered to the player.

    private void OnTriggerEnter(Collider other)
    {

            if (RegisterHit)
            {
                if (other.transform.tag == "Player")
                {
                    Vector3 hitfromdirection = other.transform.position - transform.root.transform.position;

                    other.transform.GetComponent<PlayerController>().RecieveHit(knockbackStrength, hitfromdirection, Damage);
                    transform.root.SendMessage("Hit", hitAudio);
                }

            }
    }

    //Whether or not the hit can be made

    public void RegisterOn()
    {
        RegisterHit = true;
    }

    public void RegisterOff()
    {
        RegisterHit = false;
    }

}

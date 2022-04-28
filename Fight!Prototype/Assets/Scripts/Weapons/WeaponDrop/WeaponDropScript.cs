using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropScript : MonoBehaviour
{
    //components
    public GameObject[] WeaponModels;
    public GameObject[] WeaponPrefabs;
    public Light Light;

    //Gravity Variables
    private float verticalVelocity;
    private float gravity = 5f;

    //Ground Check
    public GameObject groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    bool isGrounded;

    //Weapon Variables
    public GameObject WeaponModel;
    public GameObject WeaponPrefab;

    public int Rarity;
    //Rarity, Common 0, Rare 1, Legendary 3

    public enum Weapon

    {
        hammer,
        bat
    }

    public Weapon _weapons;

    public void SwitchWeapon(int weaponint)
    {
        _weapons = (Weapon)weaponint;
    }

    // Start is called before the first frame update
    void Start()
    {

        switch (_weapons)
        {
            case Weapon.hammer:;
                WeaponPrefab = WeaponPrefabs[0];
                WeaponModel = WeaponModels[0];
                Rarity = 1;
                break;

            case Weapon.bat:
                WeaponPrefab = WeaponPrefabs[1];
                WeaponModel = WeaponModels[1];
                Rarity = 0;
                break;
        }

        Instantiate(WeaponModel, transform);

        if (Rarity == 0)
        {
            Light.color = Color.green;
        }
        else if (Rarity == 1)
        {
            Light.color = Color.blue;
        }
        else if (Rarity == 1)
        {
            Light.color = Color.yellow;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Simulate Gravity

        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);

        if (isGrounded)
        {
            transform.Rotate(0, 0.5f, 0 * Time.deltaTime);
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity -= gravity * (Time.deltaTime);
        }

        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        transform.Translate(moveVector * Time.deltaTime);

    }

    //when the weapon is picked up the weapon prefab is applied to the players equiped weapon and the player is given a copy of the weapons
    //int value in the enum so when the weapon is dropped the right weapondrop prefab can be spawned in

    public void PickedUp(PlayerController other)
    {
        if (isGrounded)
        {
            if (other.equippedWeapon != null)
            {
                other.dropWeapon();
            }

            other.equippedWeapon = WeaponPrefab;

            other.SendMessage("equipNewWeapon", (int)_weapons);

            Destroy(gameObject);
        }
    }
}

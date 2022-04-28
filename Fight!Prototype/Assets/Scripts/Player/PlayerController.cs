using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //all variables
    #region Variables


    [Range(0, 100)]
    public float Health = 100;
    [SerializeField]
    private float playerSpeed = 37f;

    //player
    Rigidbody body;
    [SerializeField]
    GameObject PlayerModel;
    public Animator anim;
    public string device;
    [SerializeField]
    CapsuleCollider PlayerCollider;
    SoundManager sound;
    GameObject[] spawnPoint;
    [SerializeField]
    float respawnInvicinilityTimer = 3;
    public int playerIndex;

    //player particles
    [SerializeField]
    ParticleSystem WalkingDust;
    [SerializeField]
    ParticleSystem DustSplash;

    //Vector2 Input
    Vector2 movementInput = Vector2.zero;
    Vector2 aimInput = Vector2.zero;
    [SerializeField]
    LayerMask mousePlane;

    //Ground Check
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    float groundDistance = 0.5f;
    [SerializeField]
    LayerMask groundMask;

    //Movement Variables
    [SerializeField]
    private float RotationSpeed = 475f;
    bool isGrounded;
    public bool isAiming = false;
    bool isRotatingWithMouse = false;
    public bool movementDisabled = false;
    Vector3 MousePosition;

    //equipment variables
    public GameObject equippedWeapon = null;
    GameObject weapon;
    
    float WeaponRotationSpeed = 0.1f;
    bool onAttackCooldown;
    Vector3 wepHeldOut = new Vector3(291.402f, -11.105f, 86.571f);
    Vector3 wepHeldIn = new Vector3(201.91f, -66.3f, 180f);

    int weaponInt;

    [SerializeField]
    GameObject weaponGrip;
    [SerializeField]
    GameObject weaponDrop;
    bool hasWeapon;

    //Gravity Variables
    private float verticalVelocity;
    [SerializeField]
    private float gravityMultiplier = 4f;
    private float fallMultiplier = 2.5f;

    #endregion

    void Start()
    {
        spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
        body = gameObject.GetComponent<Rigidbody>();
        Vector3 PlayerModelPos = new Vector3(0, -1.04f, 0);
        PlayerModel.transform.localPosition = PlayerModelPos;
        sound = transform.GetComponent<SoundManager>();
    }


    //when the player left clicks, rotate the player with the mouse
    public void OnRotateWithMouse(InputAction.CallbackContext context)
    {

        if (context.performed == true)
        {
            isRotatingWithMouse = true;
        }
        if (context.canceled == true)
        {
            isRotatingWithMouse = false;
        }
    }

    //get the mouse aim input and detect when controller users begin using the right stick and ends using the right stick

    public void OnAim(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();

    }

    //When Attack Starts

    public void OnAttack1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (hasWeapon && !onAttackCooldown)
            {

                sound.SendMessage("Swing");

                StartCoroutine(HoldWeaponOut());
                anim.CrossFade("Swing", 0.02f);
                weapon.SendMessage("RegisterOn");


                onAttackCooldown = true;
                StartCoroutine(WeaponCooldown());
            }

        }
    }

        //When Attack Ends

    public void AttackEnd()
    {
        StartCoroutine(HoldWeaponClose());
        weapon.SendMessage("RegisterOff");
    }

    //recieve attack
    public void RecieveHit(float knockback, Vector3 hitfromdirection, float damage)
    {
        StartCoroutine(InvulnerableCooldown(0.5f)); StartCoroutine(MovementStun());

        TakeDamage(damage);

        hitfromdirection.y = 0;

        body.AddForce((hitfromdirection.normalized * knockback) + (Vector3.up * (knockback/2)), ForceMode.Impulse);
        Debug.DrawRay(transform.position, (hitfromdirection.normalized * knockback) + Vector3.up * 200, Color.green, 2, false);
    }

    //Function called up taking damage

    private void TakeDamage(float damage)
    {
        Health = Health - damage;

        Health = Mathf.Clamp(Health, 0, 100);

        if (Health <= 0)
        {
            Die();
        }
    }

    //Function called upon a players death

    public void Die()
    {
        int spawn = Random.Range(0, spawnPoint.Length);
        Health = 100;

        transform.position = spawnPoint[spawn].transform.position;

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        GameObject.FindGameObjectWithTag("PlayerManager").GetComponentInChildren<DeathCounter>().addDeath(playerIndex);
        StartCoroutine(HoldAtRespawn(spawnPoint[spawn]));
        StartCoroutine(ResapawnInvulerability());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }


    //================================================
    //Weapon Pickup & Drop
    //================================================
    #region pickup & drop
    //Pick up weapon

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            pickupWeapon();
        }
    }

    //pickup nearest weapon

    void pickupWeapon()
    {
        Collider[] weapons = Physics.OverlapSphere(transform.position, 3);

        Collider nearestweapon = null;
        float closestdistance = 9999999f;

        foreach (Collider weapon in weapons)
        {

            if (weapon.transform.tag == "Weapon")
            {

                float distance = Vector3.Distance(transform.position, weapon.transform.position);

                if (distance < closestdistance)
                {
                    closestdistance = distance;
                    nearestweapon = weapon;
                }
            }
        }

        if (nearestweapon != null)
        {
            PlayerController other = this.transform.GetComponent<PlayerController>();
            nearestweapon.transform.GetComponent<WeaponDropScript>().PickedUp(other);
        }
    }

    //Equip Weapon To Weapongrip slot

    public void equipNewWeapon(int weaponint)
    {
        weaponInt = weaponint;
        hasWeapon = true;
        StartCoroutine(HoldWeaponClose());
        weapon = Instantiate(equippedWeapon, weaponGrip.transform.position, weaponGrip.transform.rotation, weaponGrip.transform);

        Physics.IgnoreCollision(weapon.GetComponent<WeaponScript>().weaponCollider, PlayerCollider);
    }

    //Drop Weapon and create a weapon drop

    public void dropWeapon()
    {
        Destroy(weapon);
        GameObject wepdrop = Instantiate(weaponDrop, transform.position, Quaternion.identity) as GameObject;
        WeaponDropScript wepdropscript = wepdrop.GetComponent<WeaponDropScript>();

        wepdropscript.SwitchWeapon(weaponInt);
    }
    #endregion

    void FixedUpdate()
    {

        //simulate gravity
        body.AddForce(Physics.gravity * body.mass * gravityMultiplier);


    if (!movementDisabled)
        {
            //player movement
            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            move = move.normalized;
            body.AddForce(movementInput.x * playerSpeed, body.velocity.y, movementInput.y * playerSpeed, ForceMode.Force);
        }

    }

    // Update is called once per frame
    void Update()
    {
    #region gravity

        //Simulate Gravity



        #endregion
        
    //================================================
    //Rotation
    //================================================
    #region movement

        //Control Rotation
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y);

        //Gamepad Controls


        if (device.Equals("Gamepad") && !movementDisabled)
        {
            if (moveDirection != Vector3.zero && aimInput == Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                PlayerModel.transform.rotation = Quaternion.RotateTowards(PlayerModel.transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            }

            if (aimInput != Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                PlayerModel.transform.rotation = Quaternion.RotateTowards(PlayerModel.transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            }
            else if (isAiming)
            {
                isAiming = false;
            }
        }

        //Keyboard Controls

        if (device.Equals("Keyboard") && !movementDisabled)

            if (moveDirection != Vector3.zero && !isRotatingWithMouse)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                PlayerModel.transform.rotation = Quaternion.RotateTowards(PlayerModel.transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            }

        if (isRotatingWithMouse)
        {
            Cursor.lockState = CursorLockMode.Confined;

            Ray ray = Camera.main.ScreenPointToRay(aimInput);
            if (Physics.Raycast(ray, out RaycastHit raycasthit, float.MaxValue, mousePlane))
            {
                MousePosition = raycasthit.point;

                Quaternion toRotation = Quaternion.LookRotation(raycasthit.point - this.transform.position, Vector3.up);
                toRotation.x = 0; toRotation.z = 0;
                PlayerModel.transform.rotation = Quaternion.RotateTowards(PlayerModel.transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            }
        }

        #endregion

    //================================================
    //particles
    //================================================
    #region particles
        //Handle Movement Particles

        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);


        if (!isGrounded)
        {
            CreateDustSplash();
            var emission = WalkingDust.emission;
            emission.rateOverDistance = 0.5f;
        }
        else
        {
            var emission = WalkingDust.emission;
            emission.rateOverDistance = 1.5f;
        }
        #endregion

    //================================================
    //animation
    //================================================
    #region animation
        //moving

        if (moveDirection != Vector3.zero && !movementDisabled)
        {
            anim.SetBool("moving", true);
        }
        else
        {
            anim.SetBool("moving", false);
        }

        //aiming

        if (!movementDisabled)
        {
            anim.SetBool("aiming", isAiming);

            anim.SetFloat("vertical", movementInput.y);
            anim.SetFloat("horizontal", movementInput.x);
        }
        else
        {
            anim.SetBool("aiming", !isAiming);

            anim.SetFloat("vertical", 0);
            anim.SetFloat("horizontal", 0);
        }

        //holding weapon

        anim.SetBool("holdingWeapon", hasWeapon);

        if (hasWeapon)
        {
            anim.SetLayerWeight(1, 1);
        }
        else
        {
            anim.SetLayerWeight(1, 0);
        }

    }
    #endregion

    //================================================
    //Lerp Weapon Stance
    //================================================
    #region Weapon Lerp

    //Runs the lerp for the weapon being extended out
    IEnumerator HoldWeaponOut()
    {
        float timeElapsed = 0;
        Quaternion WeaponGripEnd = Quaternion.Euler(wepHeldOut);

        while (timeElapsed < WeaponRotationSpeed)
        {
            weaponGrip.transform.localRotation = Quaternion.Lerp(weaponGrip.transform.localRotation, WeaponGripEnd, timeElapsed / WeaponRotationSpeed);
            timeElapsed += Time.deltaTime;

            yield return null;

        }

        weaponGrip.transform.localEulerAngles = wepHeldOut;

        yield return null;

    }

    //Runs the lerp for the weapon being retracted in

    IEnumerator HoldWeaponClose()
    {
        float timeElapsed = 0;
        Quaternion WeaponGripEnd = Quaternion.Euler(wepHeldIn);

        while (timeElapsed < WeaponRotationSpeed)
        {
            weaponGrip.transform.localRotation = Quaternion.Lerp(weaponGrip.transform.localRotation, WeaponGripEnd, timeElapsed / WeaponRotationSpeed);
            timeElapsed += Time.deltaTime;

            yield return null;

        }

        weaponGrip.transform.localEulerAngles = wepHeldIn;

        yield return null;
    }
    #endregion

    //================================================
    //Weapon Cooldown
    //================================================
    #region Weapon Cooldown
    IEnumerator WeaponCooldown()
    {
        float timeElapsed = 0;
        float CooldownTimer = weapon.GetComponent<WeaponScript>().CooldownTime;

        while (timeElapsed < CooldownTimer)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        onAttackCooldown = false;
        yield return null;
    }
    #endregion

    //Iframe Cooldown
    IEnumerator InvulnerableCooldown(float iLength)
    {

        Physics.IgnoreLayerCollision(7, 8, true);
        yield return new WaitForSeconds(iLength);
        Physics.IgnoreLayerCollision(7, 8, false);

    }

    IEnumerator MovementStun()
    {
        yield return new WaitForSeconds(0.2f);

        while (!isGrounded)
        {
            movementDisabled = true;
            yield return null;
        }
        movementDisabled = false;
        yield return null;
    }

    //Respawn Invicinility
    IEnumerator ResapawnInvulerability()
    {
        Material bodyMaterial = transform.GetComponentInChildren<SkinnedMeshRenderer>().material;

        bodyMaterial.SetFloat("_Opacity", 0.48f);
        gameObject.layer = 31;
        yield return new WaitForSeconds(respawnInvicinilityTimer);
        bodyMaterial.SetFloat("_Opacity", 1);
        gameObject.layer = 8;

    }

    IEnumerator HoldAtRespawn(GameObject spawn)
    {
        float timeElapsed = 0;

        while (timeElapsed < 0.2f)
        {
            transform.position = spawn.transform.position;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    void CreateDustSplash()
    {
        DustSplash.Play();
    }
}
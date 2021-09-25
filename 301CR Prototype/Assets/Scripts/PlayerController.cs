using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Player's rigidbody
    Rigidbody playerRB;

    public static GameObject playerModel;

    //Speed player moves at
    float moveSpeed = 45f;

    //Value for dash
    float dashSpeed = 1000f;

    //Force applied when jumping
    float jumpSpeed = 850f;

    //Jump cooldown timer
    float jumpCooldown = 1.2f;

    //Dash cooldown timer
    float dashCooldown = 2f;


    float currJump;
    float currDash;

    //Up vector
    Vector3 upVec = new Vector3(0, 1, 0);

    //Overwritten with values
    float rotationX = 0;
    float rotationY = 0;

    //Camera sensitivity
    float sens = 4;

    //Last mosue position
    Vector3 lastMouse;

    //health Text
    public Text healthTxt;
    //Round text
    public Text roundTxt;


    //Cooldown for dash
    bool canDash = true;

    //Cooldown for jump
    bool canJump = true;

    public static float playerHealth;

    float maxhealth = 100;

    public static int points;

    public Slider jumpSlid;
    public Slider dashSlid;

    public GameObject jumpcdImg;
    public GameObject dashcdImg;

    public GameObject respawnPos;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = maxhealth;

        playerModel = this.gameObject;

        //Get player rigidbody
        playerRB = GetComponent<Rigidbody>();

        //Save last mouse pos
        lastMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) - lastMouse;


        //Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        currJump = jumpCooldown;

        currDash = dashCooldown;

    }

    void FixedUpdate() 
    {
        //For debugging
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), transform.forward, Color.green);
        ///Keyboard movements
        ///
        //Forward/Backward
        if (Input.GetKey("w"))
            {
                //Dash
                if (Input.GetKey(KeyCode.LeftShift) && canDash)
                {
                    playerRB.AddForce(playerRB.transform.forward * dashSpeed);
                    canDash = false;
                }
            else 
                {
                    playerRB.AddForce(playerRB.transform.forward * moveSpeed);

                }

        }
        
        if (Input.GetKey("s"))
        {
            //Dash
            if (Input.GetKey(KeyCode.LeftShift) && canDash)
            {
                playerRB.AddForce(playerRB.transform.forward * -dashSpeed);
                canDash = false;

            }
            else 
            {
                playerRB.AddForce(playerRB.transform.forward * -moveSpeed);

            }
        }


            //Left/Right strafe

        if (Input.GetKey("a"))
        {
             //Dash
             if (Input.GetKey(KeyCode.LeftShift) && canDash)
             {
                playerRB.AddForce(Vector3.Cross(playerRB.transform.forward, upVec.normalized) * dashSpeed);
                canDash = false;

            }
            else 
             {
                playerRB.AddForce(Vector3.Cross(playerRB.transform.forward, upVec.normalized) * moveSpeed);

             }


        }
            
        if (Input.GetKey("d"))
        {
            //Dash
            if (Input.GetKey(KeyCode.LeftShift) && canDash)
            {
                //Get right vector
                playerRB.AddForce(-(Vector3.Cross(playerRB.transform.forward, upVec.normalized) * dashSpeed));
                canDash = false;

            }
            else 
            {
                //Get right vector
                playerRB.AddForce(-(Vector3.Cross(playerRB.transform.forward, upVec.normalized) * moveSpeed));
            }
   
        }

        //Jump
        if (Input.GetKey("space"))
        {
           //If jump off cooldown
           if (canJump)
           {
               playerRB.AddForce(playerRB.transform.up * jumpSpeed);
                //StartCoroutine(jumpTimer());
                canJump = false;
           }

        }

           

            ///Mouse movements
            ///


            //Get mouse movements
            float horizontal = sens * Input.GetAxis("Mouse X");
            float vertical = sens * Input.GetAxis("Mouse Y");


            //Assign and invert Y axis
            rotationX += Input.GetAxis("Mouse Y") * -sens;
            rotationY += Input.GetAxis("Mouse X") * sens;

            //Apply rotation to rigidbody
            playerRB.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);




        if (!canJump) 
        {

            jumpSlid.value = currJump;
            jumpcdImg.SetActive(true);
            currJump -= Time.deltaTime;
            if (currJump <= 0) 
            {
                canJump = true;
            }
        }
        else 
        {
            currJump = jumpCooldown;
            jumpcdImg.SetActive(false);

        }

        if (!canDash)
        {

            dashSlid.value = currDash;
            dashcdImg.SetActive(true);
            currDash -= Time.deltaTime;
            if (currDash <= 0)
            {
                canDash = true;
            }
        }
        else
        {
            dashcdImg.SetActive(false);

            currDash = dashCooldown;
        }


    }

  

    // Update is called once per frame
    void Update()
    {

        healthTxt.text = "HLTH " + playerHealth.ToString();

        //Update round txt
        roundTxt.text = "RND: " + EnemySpawner.currRound.ToString();
    }

    public void takeDmg(float dmg)
    {
        if(playerHealth > 0) 
        {
            playerHealth -= dmg;

        }
        else
        {
            Die();

        }


    }


    public void Die()
    {
        //Respawn player
        this.transform.gameObject.transform.position = respawnPos.transform.position;

        //Reset everything
        EnemySpawner.currRound = 1;

        //Find all enemies in scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            //Kill all enemies
            Destroy(enemies[i]);



        }

        //Reset health and ammo
        playerHealth = maxhealth;

        GunController.currAmmo = GunController.maxAmmo;

        //Reset cooldowns
        canDash = true;
        canJump = true;
        GunController.cooldown = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        //If player collides with outer map, end the game
        if(collision.transform.tag == "outerMap") 
        {
            Die();
        }
    }
}

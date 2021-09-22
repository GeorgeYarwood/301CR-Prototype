using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Enemies health stat
    public float health = 50f;

  

    public ParticleSystem muzzleflash;


    //Agent's goal (Set in inspector)
    public Transform goal;

    NavMeshAgent agent;

    float waittime = 1f;

    bool playsound;

    private AudioSource Gunshot;

    private Animator anim;

    public Animator gunAnim;

    //Different states for our AI
    enum States  {Idle, Walking, Attacking, Dead };

    //Stats for gun
    public float damage = 5f;
    public float range = 100f;
    public float impactForce = 50f;
    float fireRate = 0.2f;

    

    bool firstframe;

    States currState;

    //Distance player must be in to be seen by enemy
    float lookRadius = 40f;
    float attackdist = 20f;
    float walkdist;
    

    // Start is called before the first frame update
    void Start()
    {
        //Find out agent and animator
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        goal = player.transform;

        Gunshot = GetComponent<AudioSource>();

        //Start off as idle
        currState = States.Idle;

    }

    ////For debuging
    //void OnDrawGizmosSelected()
    //{
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(transform.position, lookRadius);
    //}

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(goal.position, transform.position);

        if(playsound)
        {
            waittime -= Time.deltaTime;

            if (waittime <= 0)
            {
                Gunshot.Play(0);
                muzzleflash.Play();
                ShootTarget();
                waittime = 1.25f;
            }
        }

        //For debugging
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), transform.forward, Color.green);

        //Switch between AI states
        switch (currState)
        {
            case States.Idle:
                anim.SetBool("Idle", true);
                gunAnim.SetBool("Idle", true);
                PauseNavMesh();

                if (distance <= lookRadius)
                {
                    anim.SetBool("Idle", false);
                    gunAnim.SetBool("Idle", false);
                    currState = States.Walking;
                }
                if (distance <= attackdist)
                {
                    anim.SetBool("Idle", false);
                    gunAnim.SetBool("Idle", false);
                    currState = States.Attacking;
                }
                break;

            case States.Attacking:
                anim.SetBool("Attack", true);
                gunAnim.SetBool("Attack", true);
                
                playsound = true;
                PauseNavMesh();
                FaceTarget();
                //ShootTarget();

                if (distance <= lookRadius && distance > attackdist)
                {
                    playsound = false;
                    anim.SetBool("Attack", false);
                    gunAnim.SetBool("Attack",false);
                    currState = States.Walking;
                }
                break;
            case States.Walking:
                anim.SetBool("Walk", true);
                gunAnim.SetBool("Idle", true);
                UnPauseNavMesh();
                agent.SetDestination(goal.position);
                if (distance <= attackdist)
                {
                    anim.SetBool("Walk", false);
                    gunAnim.SetBool("Idle", false);
                    //agent.SetDestination(transform.position);
                    currState = States.Attacking;
                }
                if (distance >= lookRadius)
                {
                    anim.SetBool("Walk", false);
                    gunAnim.SetBool("Idle", false);
                    currState = States.Idle;
                }
                break;
            case States.Dead:
                playsound = false;
                muzzleflash.Stop();
                break;
        }

    }

    //Make our agent face the target   
    void FaceTarget()
    {
        Vector3 direction = (goal.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    

    //Tries to hit target with raycast and then deals damage
    void ShootTarget()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), transform.forward, out hit, range))
        {
           

            Debug.Log(hit.transform.name);

            int randNum = Random.Range(1, 2);
            //Less chance of hit doing dmg to player
            if(randNum == 1)
            {
                //Takes heath from player
                Player player = hit.transform.GetComponent<Player>();
                if (player != null)
                {
                    Debug.Log("Player hit!");
                    player.TakeDmg(damage);
                }

            }

           

            ////Adds force to shot object
            //if (hit.rigidbody != null)
            //{
            //    hit.rigidbody.AddForce(-hit.normal * impactForce);
            //}
        }

    }

  


    void PauseNavMesh()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        agent.SetDestination(transform.position);

    }
    void UnPauseNavMesh()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    public void TakeDmg(float amount)
    {
        health -= amount;

        //Get 1 point for a hit if enemy is not dead
        if (!(anim.GetCurrentAnimatorStateInfo(0).IsName("Dying")))
            {
            Player.points += 1;
        }
        

        anim.SetTrigger("Hit");

        if (health <= 0f)
        {
           if(Random.Range(1,3) == 1)
            {
                SoundController.PlayClip();
                UIGif.PlayGif();
            } 
           
            currState = States.Dead;
            Die();

        }

    }

    void Die()
    {
        anim.SetBool("Die", true);
        gunAnim.SetTrigger("Die");
        PauseNavMesh();
        health = 1000f;

        //Player gets 15 points for each kill
        Player.points += 15;

        //Destroys gameobject after 10 seconds
        Destroy(gameObject, 10);
    }
}


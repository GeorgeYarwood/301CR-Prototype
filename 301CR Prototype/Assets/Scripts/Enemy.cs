using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Enemies health stat
    public float health = 50f;


    //Agent's goal (Set in inspector)
    public Transform goal;

    NavMeshAgent agent;



    bool canHit = true;

    private Animator anim;


    //Different states for our AI
    enum States  {Walking, Attacking, Dead };

    //Stats for gun
    public float damage = 0.1f;
    

    


    States currState;

    //Distance player must be in to be attacked by enemy
    float attackdist = 5f;
    

    // Start is called before the first frame update
    void Start()
    {
        //Find out agent and animator
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        goal = player.transform;


        //Start off as idle

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
        FaceTarget();



        //For debugging
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), transform.forward, Color.green);

        //Switch between AI states
        switch (currState)
        {
            

            case States.Attacking:
                
                
                anim.SetTrigger("attack");
                agent.SetDestination(agent.transform.position);

                if (distance > attackdist)
                {
                    currState = States.Walking;
                }
                break;
            case States.Walking:

                agent.SetDestination(goal.position);
                anim.SetTrigger("walk");

                if (distance <= attackdist)
                {
                    
                    currState = States.Attacking;
                }
   
                break;
            case States.Dead:
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

    
    IEnumerator waitForNextHit() 
    {
        canHit = false;
        yield return new WaitForSeconds(.5f);
        canHit = true;
    }
 

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "Player" && canHit) 
        {
            currState = States.Attacking; 

            int randNum = Random.Range(1, 2);
            //Less chance of hit doing dmg to player
            if (randNum == 1)
            {

                Debug.Log("Player hit!");
                collision.transform.GetComponent<PlayerController>().takeDmg(damage);


            }

            StartCoroutine(waitForNextHit());
        }
    }



    
  

    public void TakeDmg(float amount)
    {
        health -= amount;

        //Get 1 point for a hit if enemy is not dead
        if (!(anim.GetCurrentAnimatorStateInfo(0).IsName("Dying")))
        {
            PlayerController.points += 1;
        }
        

        anim.SetTrigger("Hit");

        if (health <= 0f)
        {

            //anim.SetTrigger("die");
            currState = States.Dead;
           

        }

    }


}


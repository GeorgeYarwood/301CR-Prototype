using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunController : MonoBehaviour
{
    //Projectile gun will fire
    public GameObject bulletProj;
    //Force the bullet will be fired at
    float shootForce = 10f;
    //Gun model that we will manipulate
    public GameObject gunModel;

    
    //Guns animator
    public Animator gunAnim;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Aim down sight it we left click
        if (Input.GetMouseButton(0))
        {
            gunAnim.SetTrigger("aimUp");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            gunAnim.SetTrigger("aimDown");
        }

    }
}

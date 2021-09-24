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

    //Time it takes to fire a shot
    float shootTime = .5f;

    bool aiming = false;
    bool canShoot = true;
    
    //Guns animator
    public Animator gunAnim;


    // Start is called before the first frame update
    void Start()
    {
        //Get gun model
        gunModel = this.gameObject;

        //Get animator
        gunAnim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //Aim down sight it we right click
        if (Input.GetMouseButton(1) && !aiming)
        {
            gunAnim.SetTrigger("aimUp");
            aiming = true;
        }
        else if (Input.GetMouseButtonUp(1) && aiming)
        {
            gunAnim.SetTrigger("aimDown");
            aiming = false;

        }

        if (Input.GetMouseButton(0) && canShoot) 
        {
            gunAnim.SetTrigger("fire");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //If we hit
            if (Physics.Raycast(ray, out hit, 50f))
            {
                if (hit.transform.tag == "enemy")
                {
                    hit.transform.gameObject.SetActive(false);
                    Destroy(hit.transform.gameObject);
                }
            }

                StartCoroutine(shootWait());
        }
        if (Input.GetKey("r")) 
        {
            gunAnim.SetTrigger("reload");
        }

    }

    IEnumerator shootWait() 
    {
        canShoot = false;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
    }
}

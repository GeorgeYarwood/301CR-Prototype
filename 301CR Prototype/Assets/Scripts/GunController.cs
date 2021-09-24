using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunController : MonoBehaviour
{
    //Projectile gun will fire
    public GameObject bulletProj;

    //Spawn point for projectile
    public Transform bulletSpawn;
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

    //Audio source
    public AudioSource gunAudio;


    // Start is called before the first frame update
    void Start()
    {
        //Get gun model
        gunModel = this.gameObject;

        //Get animator
        gunAnim = this.GetComponent<Animator>();

        //Get audio source
        gunAudio = this.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetMouseButtonDown(0) && !gunAnim.GetCurrentAnimatorStateInfo(0).IsName("gunFire"))
        {
            gunAnim.SetTrigger("fire");

            gunAudio.pitch = Random.Range(0.7f, 1f);
            gunAudio.Play();

            GameObject bulletClone = Instantiate(bulletProj, bulletSpawn.transform);
            bulletClone.GetComponent<Rigidbody>().AddForce(bulletSpawn.transform.forward * 500);
            bulletClone.transform.parent = null;
            Destroy(bulletClone, 0.5f);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //If we hit
            if (Physics.Raycast(ray, out hit, 50f))
            {
                if (hit.transform.tag == "enemy")
                {
                    //hit.transform.gameObject.SetActive(false);
                    Destroy(hit.transform.gameObject, 0.5f);
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

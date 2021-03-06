using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    public AudioSource elecAudio;
    public AudioSource reloadAudio;
    public AudioSource errAudio;
    public AudioSource reload2Audio;

    public GameObject reloadMsg;

    public GameObject elecImg1;
    public GameObject elecImg2;

    static public int maxAmmo = 7;
    static public int currAmmo;

    float coolDwn = 100f;

    float currDwn;

    static public bool cooldown = false;

    public Text ammoTxt;

    public Slider coolDwnSlid;

    public GameObject coolDwnImg;

    // Start is called before the first frame update
    void Start()
    {
        //Get gun model
        gunModel = this.gameObject;

        //Get animator
        gunAnim = this.GetComponent<Animator>();


        currAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cooldown) 
        {
            currDwn = coolDwn;
            coolDwnImg.SetActive(false);

        }


        if (cooldown) 
        {
            coolDwnSlid.value = currDwn;
            coolDwnImg.SetActive(true);
            currDwn -= Time.deltaTime;
            if (currDwn <= 0) 
            {
                cooldown = false;
            }
        }

        ammoTxt.text = "AMO: " + currAmmo.ToString();

        if(currAmmo <= 0) 
        {
            reloadMsg.SetActive(true);
        }
        else 
        {
            reloadMsg.SetActive(false);

        }
        if (Input.GetMouseButtonDown(0) && !gunAnim.GetCurrentAnimatorStateInfo(0).IsName("gunFire") && !gunAnim.GetCurrentAnimatorStateInfo(0).IsName("gunReload") && !gunAnim.GetCurrentAnimatorStateInfo(0).IsName("gunReload2") && currAmmo > 0)
        {
            currAmmo -= 1;

            gunAnim.SetTrigger("fire");

            gunAudio.pitch = Random.Range(0.4f, 1.2f);
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
        else if(Input.GetMouseButtonDown(0) && currAmmo <= 0) 
        {
            errAudio.Play();
        }

        
        if (Input.GetKeyDown("r")) 
        {
            if(Random.Range(1, 3) == 1)
            {
                gunAnim.SetTrigger("reload");
                reloadAudio.Play();

            }
            else 
            {
                gunAnim.SetTrigger("reload2");
                reload2Audio.Play();
            }

            currAmmo = maxAmmo;
        }

        if (Input.GetKeyDown("q") && !cooldown) 
        {
            //Play audio
            elecAudio.Play();
            cooldown = true;
            StartCoroutine(electricImg());
           
        }
        else if(Input.GetKeyDown("q"))
        {
            errAudio.Play();

        }

    }


 
    

    void killEnemies() 
    {
        //Find all enemies in scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            //Kill all enemies
            Destroy(enemies[i]);

            

        }
    }

  
    IEnumerator shootWait() 
    {
        canShoot = false;
        yield return new WaitForSeconds(shootTime);
        canShoot = true;
    }

    IEnumerator electricImg() 
    {
        elecImg1.SetActive(true);
        yield return new WaitForSeconds(1);
        killEnemies();
        elecImg1.SetActive(false);
        elecImg2.SetActive(true);
        yield return new WaitForSeconds(1);
        killEnemies();

        elecImg2.SetActive(false);
        elecImg1.SetActive(true);
        yield return new WaitForSeconds(1);
        killEnemies();

        elecImg1.SetActive(false);


    }
}

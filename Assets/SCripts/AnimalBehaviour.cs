using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AnimalBehaviour : MonoBehaviour
{
    public int vida;
    Animator Controller;
    GameObject Aldeano;
    Collider deteccion;
    public bool enPersecusion;
    AldeanoBehaviour aldeanoAccess;
    float vel;
    bool enemigoCerca;
    public bool debugRango;
    public string debugObjetivo;
    public int debugVida;
    public float debugDistancia;
    Rigidbody animalRB;
    public bool vivo;
    public float duracionclip;
    public AnimationClip clip;



    // Start is called before the first frame update
    void Start()
    {
        vivo = true;
        animalRB = GetComponent<Rigidbody>();
        Controller = GetComponent<Animator>();
        deteccion = GetComponent<SphereCollider>();
        duracionclip = clip.length;
    }

    // Update is called once per frame
    void Update()
    {

        //Si no tiene Vida, se muere.
        if (vida == 0)
        {
            animalRB.isKinematic = true;
            StartCoroutine(Morir());
            vivo = false;
        }
        if (debugRango == true)
        {
            StartCoroutine(Atacar());
        }
        if (enemigoCerca == true)
        {
            vel = 1.5f;
            Controller.SetBool("Running", true);
        }
        else
        {
            enemigoCerca = false;
            vel = 1f;
            Controller.SetBool("Running", false);
        }


        if(vida < -1)
        {
            vida = 0;
        }
    }

    void OnTriggerStay(Collider detectado)
    {
        if (detectado.gameObject.transform.tag == "Persona")
        {
            Aldeano = detectado.gameObject;
            aldeanoAccess = Aldeano.GetComponent<AldeanoBehaviour>();
            enPersecusion = true;
            //Debug.Log("Persiguiendo");
            debugObjetivo = Aldeano.transform.name;
            debugVida = aldeanoAccess.vida;
            debugDistancia = Vector3.Distance(transform.position, Aldeano.transform.position);
            enemigoCerca = true;

            if (enPersecusion == true && Vector3.Distance(transform.position, Aldeano.transform.position) > 1 && vida > 0 && aldeanoAccess.vivo == true)
            {
                gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(Aldeano.transform.position - transform.position, Vector3.up)), vel * Time.deltaTime);
                Controller.SetBool("Moving", true);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Aldeano.transform.position, vel * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, Aldeano.transform.position) < 1.5)
            {
                Controller.SetBool("Moving", false);
                debugRango = true;
                Debug.Log("Listo para Atacar");
            }

            if (aldeanoAccess.vivo == false)
            {
                debugRango = false;
                enPersecusion = false;
                enemigoCerca = false;
                StopCoroutine(Atacar());
            }
        }
    }

        void OnTriggerExit (Collider Detectado)
    {
        Controller.SetBool("Moving", false);
        enPersecusion = false;
        enemigoCerca = false;
    }

    //Ataca
    IEnumerator Atacar()
    {
        yield return new WaitForEndOfFrame();
        Controller.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(2f);
        yield return new WaitForEndOfFrame();
        yield return null;
    }

    //Ejecuta Muerte.
    IEnumerator Morir()
    {
        Controller.SetTrigger("Dead");
        yield return new WaitForSeconds(duracionclip);
        vida = -1;
        Destroy(this.gameObject);
        Controller.ResetTrigger("Dead");
        
        
        yield return null;
    }

    public void HacerDaño()
    {
        if (aldeanoAccess.vivo == true)
        {
            aldeanoAccess.vida--;
            Debug.Log("Ataque Exitoso" + gameObject.name);
        }
    }

}

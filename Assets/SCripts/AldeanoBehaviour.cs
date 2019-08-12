using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AldeanoBehaviour : MonoBehaviour
{
    public int vida;
    Animator Controller;
    public GameObject[] Destino;
    int conciencia;
    public float vel;
    public bool enemigoCerca;
    public bool actividadTerminada;
    public bool destinoAlcanzado;
    public string debugStatus;
    public int debugExcepto;
    public float debugDistancia;
    public Vector3 destinoPos;
    public string debugDestino;
    int i;
    public bool debugEscogido;
    AnimalBehaviour animalAccess;
    Rigidbody aldeanoRB;
    public bool enCombate;
    GameObject Presa;
    public bool enRango;
    public bool vivo;
    SphereCollider sentidos;
    public string debugObjetivo;
    public int debugVida;
    public float duracionclip;
    public AnimationClip clip;

    // Start is called before the first frame update
    void Start()
    {
        duracionclip = (float)clip.length;
        Controller = GetComponent<Animator>();
        actividadTerminada = false;
        conciencia = Random.Range(1, 3);
        debugExcepto = conciencia;
        aldeanoRB = GetComponent<Rigidbody>();
        sentidos = GetComponent<SphereCollider>();
        vivo = true;
    }

    // Update is called once per frame
    void Update()
    {

        //Si no tiene Vida, se muere.
        if (vida == 0)
        {
            aldeanoRB.isKinematic = true;
            StartCoroutine(Morir());
            vivo = false;
        }

        //Si hay enemigos cerca, debe correr.
        if (enemigoCerca == true)
        {
            vel = 1.5f;
            Controller.SetBool("Running", true);
        }
        else
        {
            enemigoCerca = false;
            vel = 1;
            Controller.SetBool("Running", false);
        }
        if(enRango == true)
        {
            StartCoroutine(Atacar());
        }

    }

    void FixedUpdate()
    {
        //Si termino la actividad, decide que hacer ahora.
        if (actividadTerminada == true)
        {
            RandomExcept(1, 4, debugExcepto);
            debugEscogido = false;
        }

        if (enCombate == false)
            {
            // Que hacer.
            switch (conciencia)
            {
                case 1: //Plantar
                    debugStatus = "Sembrando";
                    StartCoroutine(Plantar());
                    break;

                case 2: //Recolectar
                    debugStatus = "Recolectando";

                    StartCoroutine(Recolectar());
                    break;

                case 3: //Descansar
                    debugStatus = "Descansando";
                    StartCoroutine(Descansar());
                    break;

                default:
                    break;
            }
        }
    }

    void OnTriggerStay(Collider detectado)
    {
        if (detectado.gameObject.transform.tag == "Animal")
        {
            Presa = detectado.gameObject;
            animalAccess = Presa.GetComponent<AnimalBehaviour>();
            enCombate = true;
            //Debug.Log("Persiguiendo");
            debugObjetivo = Presa.transform.name;
            debugVida = animalAccess.vida;
            debugDistancia = Vector3.Distance(transform.position, Presa.transform.position);
            enemigoCerca = true;

            if (enCombate == true && Vector3.Distance(transform.position, Presa.transform.position) > 1 && vida > 0 && animalAccess.vivo == true)
            {
                gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(Presa.transform.position - transform.position, Vector3.up)), vel * Time.deltaTime);
                Controller.SetBool("Moving", true);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Presa.transform.position, vel * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, Presa.transform.position) < 1.5)
            {
                Controller.SetBool("Moving", false);
                enRango = true;
                Debug.Log("Listo para Atacar");
            }

            if (animalAccess.vivo == false)
            {
                enRango = false;
                enCombate = false;
                enemigoCerca = false;
                StopCoroutine(Atacar());
                actividadTerminada = true;
            }
        }

    }

    void OnTriggerExit(Collider Detectado)
    {
        Controller.SetBool("Moving", false);
        enCombate = false;
        enemigoCerca = false;

    }

    //Ataca
    IEnumerator Atacar()
    {
        enRango = false;
        yield return new WaitForEndOfFrame();
        Controller.SetTrigger("Attack");
        yield return new WaitForSeconds(3.27f);
        Controller.ResetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        enRango = true;
        yield return new WaitForEndOfFrame();
        yield return null;
    }

    //Sembrar
    IEnumerator Plantar()
    {
        actividadTerminada = false;
        yield return new WaitForSeconds(.5f);
        Destino = GameObject.FindGameObjectsWithTag("Campo");
        if (debugEscogido == false)
        {
            yield return new WaitForEndOfFrame();
            RandomInArray(0,Destino.Length);
            yield return new WaitForEndOfFrame();
        }
        
//Debug.Log("Caminando a" + Destino[i].name);
        destinoPos = Destino[i].transform.position;
        debugDistancia = Vector3.Distance(transform.position, Destino[i].transform.position);
        debugDestino = Destino[i].name;
        

        if (Vector3.Distance(transform.position, Destino[i].transform.position + new Vector3(Destino[i].transform.position.x + Random.Range(-1, 1), Destino[i].transform.position.y, Destino[i].transform.position.z + Random.Range(-1, 1))) > 1 && vida > 0)
        {
            //Debug.Log("En movimiento");
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(Destino[i].transform.position - transform.position,Vector3.up)), 2 * Time.deltaTime);
            yield return new WaitForSeconds(1f);
            Controller.SetBool("Moving", true);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Destino[i].transform.position, vel * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, Destino[i].transform.position) <= 1)
        {
            conciencia = 0;
            actividadTerminada = false;
            yield return new WaitForSeconds(.5f);
            //Debug.Log("Sembrando");
            Controller.SetTrigger("Plant");
            yield return new WaitForEndOfFrame();
            Controller.ResetTrigger("Plant");
            yield return new WaitForSeconds(7f);
            actividadTerminada = true;
        }
        yield return null;
    }

    //Recoger Fruta
    IEnumerator Recolectar()
    {
        actividadTerminada = false;
        yield return new WaitForSeconds(.5f);
        Destino = GameObject.FindGameObjectsWithTag("Arbol_Frutal");
        if (debugEscogido == false)
        {
            yield return new WaitForEndOfFrame();
            RandomInArray(0, Destino.Length);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Caminando a" + Destino[i].name);       
        destinoPos = Destino[i].transform.position;
        debugDistancia = Vector3.Distance(transform.position, Destino[i].transform.position);
        debugDestino = Destino[i].name;


        if (Vector3.Distance(transform.position, Destino[i].transform.position) > 1 && vida > 0)
        {
            //Debug.Log("En movimiento");
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(Destino[i].transform.position - transform.position, Vector3.up)), 2 * Time.deltaTime);
            yield return new WaitForSeconds(1f);
            Controller.SetBool("Moving", true);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Destino[i].transform.position, vel * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, Destino[i].transform.position) <= 1) 
        {
            conciencia = 0;
            actividadTerminada = false;
            yield return new WaitForSeconds(.5f);
            //Debug.Log("Recolectando");
            Controller.SetTrigger("Harvest");
            yield return new WaitForEndOfFrame();
            Controller.ResetTrigger("Harvest");
            yield return new WaitForSeconds(8f);
            actividadTerminada = true;
        }
        yield return null;
    }

    //Descansar
    IEnumerator Descansar()
    {
        actividadTerminada = false;
        yield return new WaitForSeconds(.5f);
        Destino = GameObject.FindGameObjectsWithTag("Fogata");
        if (debugEscogido == false)
        {
            yield return new WaitForEndOfFrame();
            RandomInArray(0, Destino.Length);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Caminando a" + Destino[i].name);
        destinoPos = Destino[i].transform.position;
        debugDistancia = Vector3.Distance(transform.position, Destino[i].transform.position);
        debugDestino = Destino[i].name;


        if (Vector3.Distance(transform.position, Destino[i].transform.position) > 4 && vida > 0)
        {
            //Debug.Log("En movimiento");
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(Destino[i].transform.position - transform.position, Vector3.up)), 2 * Time.deltaTime);
            yield return new WaitForSeconds(1f);
            Controller.SetBool("Moving", true);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Destino[i].transform.position, vel * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, Destino[i].transform.position) <= 4)
        {
            conciencia = 0;
            actividadTerminada = false;
            yield return new WaitForSeconds(.5f);
            //Debug.Log("Descansando");
            Controller.SetTrigger("Sit");
            yield return new WaitForEndOfFrame();
            Controller.ResetTrigger("Sit");
            yield return new WaitForSeconds(15);
            yield return new WaitForEndOfFrame();
            actividadTerminada = true;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }


    //Ejecuta Muerte.
    IEnumerator Morir()
    {
        Controller.SetTrigger("Dead");
        yield return new WaitForSeconds(duracionclip);
        vida = -1;
        Controller.ResetTrigger("Dead");
        StopCoroutine(Atacar());
        Destroy(this.gameObject);
        yield return null;
    }

    //Genera un numero aleatorio exceptuando valor actual.
    public int RandomExcept(int min, int max, int except)
    {
        StopAllCoroutines();
        actividadTerminada = false;
        int result = Random.Range(min, max - 1);
        if (result == except) result += 1;
        if (result > 4) result = 4;
        debugExcepto = result;
        return conciencia = result;
    }

    public int RandomInArray(int min, int max)
    {
        debugEscogido = true;
        int result = Random.Range(min, max);
        return i = result;
    }

    public void HacerDaño()
    {
        if (animalAccess.vivo == true)
        {
            animalAccess.vida--;
            Debug.Log("Ataque Exitoso" + gameObject.name);
        }
    }


}

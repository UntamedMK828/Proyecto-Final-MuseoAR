using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public GameObject Sol;
    public Light solProperties;
    public Color[] colorLuz = new Color[3];
    public bool lloviendo;
    public bool nevando;
    public bool soleado;
    public GameObject lluvia;
    public GameObject nieve;
    GameObject climaActual;
    GameObject animales;

    // Start is called before the first frame update
    void Start()
    {
        solProperties = Sol.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        climaActual = GameObject.FindGameObjectWithTag("Clima");
        switch (climaActual.name)
        {
            case "Lluvia":
                Debug.Log(climaActual.name + " detectado");
                nieve.SetActive(false);
                solProperties.color = Color.Lerp(solProperties.color, colorLuz[2], 3);
                break;

            case "Sol":
                Debug.Log(climaActual.name + " detectado");
                nieve.SetActive(false);
                lluvia.SetActive(false);
                solProperties.color = Color.Lerp(solProperties.color, colorLuz[0],3);
                break;

            case "Nieve":
                Debug.Log(climaActual.name + " detectado");
                lluvia.SetActive(false);
                solProperties.color = Color.Lerp( solProperties.color, colorLuz[1],3 );
                break;

            default:
                Debug.Log("Sin Clima");
                solProperties.color = colorLuz[0];
                break;

        }
        animales = GameObject.FindGameObjectWithTag("Animales");
        switch (animales.name)
        {
            case "Tigres":
                Debug.Log(animales.name + " detectados");
                break;

            case "Lobos":
                Debug.Log(animales.name + " detectados");
                break;
        }
    }
}

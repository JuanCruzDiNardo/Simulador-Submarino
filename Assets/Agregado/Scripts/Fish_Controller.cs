using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Fish_Controller : MonoBehaviour
{
    public species species; 
    public int FishPoints = 10; //Puntaje por pez
    public float swimSpeed = 2f; // Velocidad normal de nado
    public float escapeSpeed = 5f; // Velocidad al huir
    //public float detectionRadius = 5f; // Distancia de detección del jugador
    public bool escaping = false;
    public bool alredyCaptured = false;

    private Vector3 randomDirection;
    private float changeDirectionInterval = 5f; // Tiempo entre cambios de dirección
    private float directionChangeTimer;
    Vector3 direction = Vector3.zero;
    float speed = 0;
    private Transform player;
    private Rigidbody rb;
    public WaterSurface waterSurface;

    void Start()
    {
        // Asigna una dirección inicial aleatoria
        ChooseRandomDirection();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        waterSurface = FindAnyObjectByType<WaterSurface>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {        
        // Actualiza el temporizador de cambio de dirección si no está escapando
        if (!escaping)
        {
            directionChangeTimer += Time.deltaTime;
            if (directionChangeTimer >= changeDirectionInterval)
            {
                ChooseRandomDirection();
                directionChangeTimer = 0;
                changeDirectionInterval = Random.Range(2f, 7f);
                direction = randomDirection;
                speed = swimSpeed;
            }            
        }
        else
        {
            // Nadar en dirección contraria al jugador
            Vector3 escapeDirection = (transform.position - player.position).normalized;
            direction = escapeDirection;
            speed = escapeSpeed;
            directionChangeTimer = 0;
        }
        Swim(direction, speed);
        RestrictDepth();
    }

    private void Swim(Vector3 direction, float speed)
    {
        // Aplicar movimiento en la dirección dada usando el Rigidbody
        rb.velocity = direction * speed;

        // Rotar gradualmente el pez hacia la dirección de movimiento
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * speed/2);
        }
    }

    private void ChooseRandomDirection()
    {
        // Elegir una dirección aleatoria para nadar
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public float GetWaterHeightAtPosition()
    {
        Vector3 position = transform.position;

        // Parametros de busqueda
        WaterSearchParameters searchParameters = new WaterSearchParameters();
        WaterSearchResult searchResult = new WaterSearchResult();

        if (waterSurface != null)
        {
            // Configuracion de los parametros de busqueda 
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Se proyecta la posicion de la superficie del agua basada en la busqueda y se guarda el vector3 de posicion en el resultado
            if (waterSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                return searchResult.projectedPositionWS.y;
            }
        }

        return 0f;
    }

    void RestrictDepth()
    {
        // Obtener la altura del agua en la posición actual del submarino
        float waterHeight = GetWaterHeightAtPosition();

        // Restringir la altura del submarino para que no salga del agua
        if (transform.position.y > waterHeight)
        {
            // Mantener la posición Y del submarino en el nivel máximo permitido
            transform.position = new Vector3(transform.position.x, waterHeight, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        // Cuando el jugador entra en el trigger, activar escape
        if (trigger.CompareTag("Player"))
        {
            escaping = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        // Cuando el jugador sale del trigger, desactivar escape
        if (trigger.CompareTag("Player"))
        {
            escaping = false;
            directionChangeTimer = 0;
        }
    }
}

public class fish
{
    public species FishSpecies { get; set; }

    public int FishPoints { get; set; }
}

public enum species
{
    Shark,
    hake,
    turbot,
    trout,
    salmon
}
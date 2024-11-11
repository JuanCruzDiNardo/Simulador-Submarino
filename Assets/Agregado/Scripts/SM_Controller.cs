using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SM_Controller : MonoBehaviour
{

    public float rotationSpeed = 30f;  // Velocidad de rotación del submarino
    public float moveSpeed = 10f;      // Velocidad base de movimiento del submarino
    public float accelerationSpeed = 20f;   // Velocidad cuando se presiona la barra espaciadora
    public float currentSpeed;          // velocidad actual
    public float straightenSpeed = 1f; // Velocidad con la que se endereza el submarino
    public float emergeSpeed = 10f;    //velocidad a la que emerge el submarino
    public WaterSurface waterSurface;  // Referencia al componente WaterSurface
    public bool isEmerging = false;     // Indica si el modo "Emerger" esta activado
    public Rigidbody rb;

    public Screenshot_Controller Camera;
    


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Camera = GetComponentInChildren<Screenshot_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isEmerging)
        {
            movement();
            acceleration();
            RestrictSubmarineDepth();
            TakePhoto();
        }
        else
        {
            emerge();
        }

        emergeMode();
    }

    private void TakePhoto()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Camera.PhotoMode();
            //Camera.UI.SetActive(true);        
        }else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Camera.Photo();
        }        
           
    }

    private void emergeMode()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isEmerging = true;
            //isEmerging = !isEmerging;
        }
    }

    private void emerge()
    {
        // Obtener la altura del agua en la posición actual
        float waterHeight = GetWaterHeightAtPosition();

        // Valida la altura del submarino
        if (transform.position.y < waterHeight)
        {
            // Mover hacia arriba en línea recta
            rb.velocity = Vector3.up * emergeSpeed;

            // Eliminar cualquier rotación y mantenerlo recto
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            rb.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, straightenSpeed * Time.deltaTime);
        }
        else
        {
            // Detener el proceso de emerger una vez que alcanza la superficie
            isEmerging = false;

            //floating();
        }
    }

    //private void floating()
    //{
    //    transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, GetWaterHeightAtPosition(), 0.05f), transform.position.z);
    //}

    private void acceleration()
    {
        // Movimiento hacia adelante
        if (Input.GetKey(KeyCode.Space))
        {
            // Acelerar cuando se presiona la barra espaciadora
            currentSpeed = accelerationSpeed;
        }
        else
        {
            // Velocidad normal
            currentSpeed = moveSpeed;
        }

        // Mover hacia adelante en la dirección en que está mirando el submarino
        //transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        rb.velocity = (transform.forward * currentSpeed );
        //Debug.Log(rb.velocity);
    }

    void movement()
    {
        // Rotación con W A S D
        float horizontal = Input.GetAxis("Horizontal"); // A y D
        float vertical = Input.GetAxis("Vertical");     // W y S

        if (horizontal != 0 || vertical != 0)
        {
            // Rotar sobre el eje Y (horizontal) y el eje X (vertical)
            //transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime);   // Rotación sobre el eje Y (izquierda y derecha)
            //transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime);  // Rotación sobre el eje X (arriba y abajo)
            
            rb.MoveRotation( rb.rotation * Quaternion.Euler((vertical * rotationSpeed * Time.deltaTime), (horizontal * rotationSpeed * Time.deltaTime), 0));
        }
        else
        {
            // Enderezar el submarino si no está rotando (en los ejes Y y Z)
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, straightenSpeed * Time.deltaTime);
        }
    }

    void RestrictSubmarineDepth()
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            //rb.velocity += Vector3.up * emergeSpeed * 3;
        }
    }
}


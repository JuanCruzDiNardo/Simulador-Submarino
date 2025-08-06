using TMPro;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Screenshot_Controller : MonoBehaviour
{
    // Define el nombre del archivo base
    public string screenshotFileName = "SubmarineScreenshot";
    public int screenshotCount = 0;  // Contador para numerar las capturas
    public GameObject UI;
    public GameObject CameraUI;    
    public TextMeshProUGUI txtPoints;

    public List<Fish_Controller> fishInCamera = new List<Fish_Controller>();

    public static List<fish> fishCaught = new List<fish>();

    private int fishCount = 0;              // Cantidad de peces en el área
    //public int pointsPerFish = 10;          // Puntos por cada pez en el área

    //public int pointsInCamera = 0;          // Puntos actuales en camara sin tomar la foto
    public static int totalPoints = 0;            // Puntos acumulados
    
    public static bool isPhotoMode = false;       // Indica si el jugador está en "modo foto"

    public SubmarineSoundController soundManager;     // Audio State Manager

    // Llama a esta función para tomar la captura de pantalla
    public void PhotoMode()
    {
        // Activar o desactivar el modo foto con la tecla P
        isPhotoMode = !isPhotoMode;     // Alternar entre activar y desactivar el modo foto
        Debug.Log(isPhotoMode ? "Modo foto activado" : "Modo foto desactivado");
        UI.SetActive(!isPhotoMode);
        CameraUI.SetActive(isPhotoMode);
    }

    public void Photo()
    {
        if (isPhotoMode)
        {
            //UI.SetActive(false);
            TakeScreenshot();
            soundManager.FlashSound();
            //UI.SetActive(true);
        }
    }

    private void TakeScreenshot()
    {        
        // Incrementa el contador de capturas y genera el nombre del archivo
        screenshotCount++;
        string fileName = screenshotFileName + "_" + screenshotCount + ".png";
        string folderPath = Path.Combine(Application.persistentDataPath, "Screenshots");

        // Crea la carpeta de capturas si no existe
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Ruta completa de la captura
        string filePath = Path.Combine(folderPath, fileName);
       
        // Captura y guarda la pantalla
        ScreenCapture.CaptureScreenshot(filePath);        

        //Debug.Log("Captura de pantalla guardada en: " + filePath);
        
        CountPoints();
    }

    private void CountPoints()
    {
        // Calcular puntos si hay peces en el área
        if (fishCount > 0)
        {
            int pointsEarned = 0;
            foreach (Fish_Controller fish in fishInCamera)
            {
                fish.alredyCaptured = true;
                pointsEarned += fish.FishPoints; //fishCount * pointsPerFish;

                fishCaught.Add(new fish() { FishSpecies = fish.species, FishPoints = fish.FishPoints });
            }                           
            totalPoints += pointsEarned;
            Debug.Log("Points earned: " + pointsEarned + " | Total points: " + totalPoints);

            txtPoints.text = "Points earned: " + pointsEarned + " | Total points: " + totalPoints;

            fishInCamera.Clear();
        }
        else
        {
            Debug.Log("No hay peces en el área de detección.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto en el trigger es un pez
        if (other.CompareTag("Fish"))
        {
            fishCount++;                    // Incrementa el contador de peces en el área

            Fish_Controller fish = other.gameObject.GetComponent<Fish_Controller>();

            if (!fish.alredyCaptured)
            {                                
                fishInCamera.Add(fish);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica si un pez sale del área de foto
        if (other.CompareTag("Fish"))
        {
            fishCount--;                    // Decrementa el contador de peces en el área

            Fish_Controller fish = other.gameObject.GetComponent<Fish_Controller>();

            if (!fish.alredyCaptured)
            {                                
                fishInCamera.Remove(fish);
            }
        }
    }

}

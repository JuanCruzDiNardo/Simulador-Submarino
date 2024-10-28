using UnityEngine;
using System.IO;

public class Screenshot_Controller : MonoBehaviour
{
    // Define el nombre del archivo base
    public string screenshotFileName = "SubmarineScreenshot";
    public int screenshotCount = 0;  // Contador para numerar las capturas
    public GameObject UI;

    // Llama a esta función para tomar la captura de pantalla
    public void Photo()
    {
        UI.SetActive(false);
        TakeScreenshot();
        UI.SetActive(true);
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

        Debug.Log("Captura de pantalla guardada en: " + filePath);        
    }
}

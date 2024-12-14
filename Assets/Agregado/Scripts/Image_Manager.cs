using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Image_Manager : MonoBehaviour
{
    private Canvas uiCanvas; // Asigna el Canvas desde el Inspector    
    private string screenshotFolderPath;    

    public void LoadGallery()
    {
        uiCanvas = GetComponent<Canvas>();

        // Obtener todas las imágenes del Canvas
        List<Image> imageList = GetAllImagesFromCanvas(uiCanvas);

        // Ejemplo de uso: imprimir el número de imágenes encontradas
        Debug.Log("Número de imágenes encontradas: " + imageList.Count);

        screenshotFolderPath = Path.Combine(Application.persistentDataPath, "Screenshots");

        // Cargar y mostrar las últimas 3 capturas
        LoadAndDisplayScreenshots(imageList);
    }

    private List<Image> GetAllImagesFromCanvas(Canvas canvas)
    {
        // Inicializa la lista para almacenar las imágenes
        List<Image> images = new List<Image>();        

        // Obtiene todos los GameObjects hijos del Canvas
        foreach (Transform child in canvas.transform)
        {
            // Verifica si el GameObject tiene un componente Image
            Image img = child.GetComponent<Image>();            

            // Si tiene Image y NO tiene Button, lo agrega a la lista
            if (img != null && child.CompareTag("Image"))
            {
                img.enabled = true;
                images.Add(img);
            }
        } 

        return images;
    }
    
    private void LoadAndDisplayScreenshots(List<Image> images)
    {
        // Obtiene todos los archivos de la carpeta y ordena por fecha de modificación (recientes primero)
        if (Directory.Exists(screenshotFolderPath))
        {
            string[] screenshotFiles = Directory.GetFiles(screenshotFolderPath, "*.png");
            List<string> latestScreenshots = new List<string>(screenshotFiles);
            latestScreenshots.Sort((x, y) => File.GetLastWriteTime(y).CompareTo(File.GetLastWriteTime(x)));

            // Cargar las últimas imágenes
            for (int i = 0; i < images.Count; i++)
            {
                if (i < latestScreenshots.Count)
                {
                    // Cargar la imagen desde el archivo y mostrarla
                    string filePath = latestScreenshots[i];
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(fileData);

                    // Convertir a Sprite y asignar a la UI.Image
                    Sprite screenshotSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    images[i].sprite = screenshotSprite;
                }
                else
                {
                    // Si no hay suficientes capturas, deja la UI.Image vacía
                    images[i].sprite = null;
                    images[i].enabled = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la carpeta de capturas de pantalla.");
        }
    }
}

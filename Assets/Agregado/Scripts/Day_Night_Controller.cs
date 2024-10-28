using System;
using UnityEngine;

public class Day_Night_Controller : MonoBehaviour
{
    [Header("Sun Light")]
    public Light sunLight;         // Luz direccional que representa el sol

    [Header("Moon Light")]
    public Light moonLight;        // Luz direccional para representar la luna
    //public Color moonColor = new Color(0.5f, 0.5f, 0.6f); // Color frío para la luna

    [Header("Time Settings")]
    [Range(0, 24)]    
    public static float timeOfDay = 12f;  // Hora del día en formato de 24 horas
    public float dayDuration = 60f; // Duración de un día completo en segundos
    public static bool isCycling = false;

    void Start()
    {
        // Configura el color y la intensidad de la luz de la luna (fijo)
        //moonLight.color = moonColor;
        moonLight.intensity = 0.2f; // Puedes ajustar este valor para controlar la intensidad de la luna
    }

    void Update()
    {
        if (!isCycling)
            return;

        TimeCycle();
        SunMoonRotation();
        DayOnOff();

    }

    private void TimeCycle()
    {
        // Actualizar la hora del día
        timeOfDay += (Time.deltaTime / dayDuration) * 24f;
        if (timeOfDay >= 24f) timeOfDay = 0f; // Reiniciar al llegar a 24 horas
    }

    private void DayOnOff()
    {
        // Activar/desactivar la luz del sol y de la luna según la hora del día
        if (timeOfDay >= 6f && timeOfDay <= 18f) // De 6:00 a 18:00 es de día
        {
            sunLight.enabled = true;
            moonLight.enabled = false;
        }
        else // De 18:00 a 6:00 es de noche
        {
            sunLight.enabled = false;
            moonLight.enabled = true;
        }
    }

    private void SunMoonRotation()
    {
        // Calcular el ángulo de rotación para el sol y la luna
        float sunAngle = (timeOfDay / 24f) * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0);

        // Ajustar la rotación opuesta para la luna (180 grados respecto al sol)
        float moonAngle = sunAngle + 180f;
        moonLight.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0);
    }
}

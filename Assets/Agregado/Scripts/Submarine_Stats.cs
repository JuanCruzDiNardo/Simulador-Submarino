using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class SubmarineStats : MonoBehaviour
{
    public SM_Controller submarine;     // Referencia al submarino
    public SubmarineSoundController soundManager;     // Audio State Manager
    public WaterSurface waterSurface; // Referencia a la superficie del agua (WaterSurface)
    public TextMeshProUGUI depthText;          // Texto UI para mostrar la profundidad
    public TextMeshProUGUI pressureText;       // Texto UI para mostrar la presión
    public TextMeshProUGUI oxygenText;         // Texto UI para mostrar el oxígeno
    public TextMeshProUGUI oxygenAlertText;         // Texto UI para mostrar el oxígeno

    public int tutorialStep = 0; // Paso actual del tutorial

    bool underwater = false;

    public float oxygen = 100f;     // Oxígeno máximo
    public float oxygenConsumptionRate = 0.2f; // Tasa de consumo base de oxígeno
    public float oxygenRechargeRate = 10f;     // Tasa de recarga de oxígeno en la superficie
    public float speedInfluenceOnOxygen = 0.05f; // Factor de influencia de la velocidad en el consumo de oxígeno

    private float pressure;         // Presión calculada
    private float depth;            // Profundidad calculada
    private float maxPressureDepth = 100f; // Profundidad máxima para calcular la presión
    private float maxPressure = 10f; // Presión máxima simulada

    void Update()
    {
        CalculateDepth();
        CalculatePressure();
        UpdateOxygen();
        UpdateUI();
        TutoStepCheck();
    }

    private void TutoStepCheck()
    {
        if (tutorialStep == 0 && depth > 20)
        {
            tutorialStep++;
        }else if (tutorialStep == 1 && pressure > 4)
        {
            tutorialStep++;
        }else if(tutorialStep == 2 && oxygen < 50)
        {
            tutorialStep++;
        }else if(tutorialStep == 3 && oxygen < 25)
        {
            tutorialStep++;
        }else if(tutorialStep == 4 && oxygen > 80)
        {
            tutorialStep++;
        }else if(tutorialStep == 5 && submarine.Camera.screenshotCount > 0)
        {
            tutorialStep++;
            Day_Night_Controller.isCycling = true;
        }else if(tutorialStep == 6 && Day_Night_Controller.timeOfDay < 12)
        {
            tutorialStep++;
        }
    }

    // Calcular la profundidad del submarino en relación a la superficie del agua
    void CalculateDepth()
    {
        float waterHeight = submarine.GetWaterHeightAtPosition();
        depth = Mathf.Max(0f, waterHeight - submarine.transform.position.y);
    }

    // Calcular la presión basada en la profundidad
    void CalculatePressure()
    {
        pressure = Mathf.Lerp(1f, maxPressure, depth / maxPressureDepth);
    }

    // Consumir o recargar oxígeno dependiendo de si el submarino está bajo el agua
    void UpdateOxygen()
    {
        if (depth > 3)
        {
            if (!underwater){
                underwater = true;
                soundManager.SubmergeSound();
                soundManager.SwitchState(SoundState.Underwater);
            }            

            // Consumir oxígeno más rápido a mayor presión
            float consumption = oxygenConsumptionRate * pressure + submarine.currentSpeed * speedInfluenceOnOxygen;
            oxygen = Mathf.Max(0, oxygen - consumption * Time.deltaTime);

            if (oxygen < 10) 
            {
                submarine.isEmerging = true;
                oxygenAlertText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Recargar oxígeno cuando esté en la superficie
            oxygen = Mathf.Min(100, oxygen + oxygenRechargeRate * Time.deltaTime);

            if (underwater)
            {
                underwater = false;
                soundManager.SubmergeSound();
                soundManager.SwitchState(SoundState.OnSurface);
            }                           
        }

        oxygenAlert();
    }

    private void oxygenAlert()
    {
        if(oxygen > 80)
            oxygenText.color = Color.green;
        else if (oxygen > 50) 
        { 
            oxygenText.color = Color.white;
            soundManager.ToggleAlarm(false);
            oxygenAlertText.gameObject.SetActive(false);
        }
        else if (oxygen > 25)
            oxygenText.color = Color.yellow;
        else
        {
            soundManager.ToggleAlarm(true);
            oxygenText.color = Color.red;
        }            
    }

    // Actualizar el UI con los valores actuales
    void UpdateUI()
    {
        depthText.text = "Depth: " + depth.ToString("F1") + " meters";
        pressureText.text = "Pressure: " + pressure.ToString("F1") + " atm";
        oxygenText.text = "Oxygen: " + oxygen.ToString("F1") + "%";
    }
}

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
    public TextMeshProUGUI pressureText;       // Texto UI para mostrar la presi�n
    public TextMeshProUGUI oxygenText;         // Texto UI para mostrar el ox�geno
    public TextMeshProUGUI oxygenAlertText;         // Texto UI para mostrar el ox�geno

    public int tutorialStep = 0; // Paso actual del tutorial

    bool underwater = false;

    public float oxygen = 100f;     // Ox�geno m�ximo
    public float oxygenConsumptionRate = 0.2f; // Tasa de consumo base de ox�geno
    public float oxygenRechargeRate = 10f;     // Tasa de recarga de ox�geno en la superficie
    public float speedInfluenceOnOxygen = 0.05f; // Factor de influencia de la velocidad en el consumo de ox�geno

    private float pressure;         // Presi�n calculada
    private float depth;            // Profundidad calculada
    private float maxPressureDepth = 100f; // Profundidad m�xima para calcular la presi�n
    private float maxPressure = 10f; // Presi�n m�xima simulada

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

    // Calcular la profundidad del submarino en relaci�n a la superficie del agua
    void CalculateDepth()
    {
        float waterHeight = submarine.GetWaterHeightAtPosition();
        depth = Mathf.Max(0f, waterHeight - submarine.transform.position.y);
    }

    // Calcular la presi�n basada en la profundidad
    void CalculatePressure()
    {
        pressure = Mathf.Lerp(1f, maxPressure, depth / maxPressureDepth);
    }

    // Consumir o recargar ox�geno dependiendo de si el submarino est� bajo el agua
    void UpdateOxygen()
    {
        if (depth > 3)
        {
            if (!underwater){
                underwater = true;
                soundManager.SubmergeSound();
                soundManager.SwitchState(SoundState.Underwater);
            }            

            // Consumir ox�geno m�s r�pido a mayor presi�n
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
            // Recargar ox�geno cuando est� en la superficie
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

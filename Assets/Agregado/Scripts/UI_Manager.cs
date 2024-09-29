using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public Canvas pauseUI;
    public Canvas inGameUI;
    public Canvas Tutorial;    

    public int TutorialStep = -1;

    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseUI.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        paused = false;               
    }

    // Update is called once per frame
    void Update()
    {
        pause();
        StepCheck();       
    }

    private void StepCheck()
    {
        if (TutorialStep != GetComponent<SubmarineStats>().tutorialStep)
        {
            TutorialStep = GetComponent<SubmarineStats>().tutorialStep;
            TutoStepChange();
        }
    }

    private void TutoStepChange()
    {        

        for (int i = 0; i < Tutorial.transform.childCount; i++)
        {
            if (i == TutorialStep)
            {
                Tutorial.transform.GetChild(i).gameObject.SetActive(true);
            }else
            {
                Tutorial.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //if (TutorialStep == 0)
        //{            
        //    Tutorial.transform.GetChild(0).gameObject.SetActive(true);
        //}else if (TutorialStep == 1)
        //{
        //    Tutorial.transform.GetChild(1).gameObject.SetActive(true);
        //}else if(TutorialStep == 2)
        //{
        //    Tutorial.transform.GetChild(2).gameObject.SetActive(true);
        //}else if(TutorialStep == 3)
        //{
        //    Tutorial.transform.GetChild(3).gameObject.SetActive(true);
        //}
    }

    // Función para reanudar el juego
    public void ResumeGame()
    {
        Time.timeScale = 1f;           // Restablecer el tiempo del juego
        inGameUI.gameObject.SetActive(true);      // Activar el Canvas de juego
        pauseUI.gameObject.SetActive(false);      // Desactivar el Canvas de pausa
    }

    // Función para ir al menú principal
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;           // Restablecer el tiempo (en caso de estar pausado)
        SceneManager.LoadScene("MainMenu"); // Cargar la escena del menú principal
    }

    // Función para salir del juego
    public void QuitGame()
    {
        Debug.Log("Quit Game!");       // Esto se verá en el editor
        Application.Quit();            // Cerrar el juego (solo funciona en la build)
    }

    private void pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {                
                pauseUI.gameObject.SetActive(false);
                inGameUI.gameObject.SetActive(true);
                paused = false;
                Time.timeScale = 1f;
            }
            else
            {                
                pauseUI.gameObject.SetActive(true);
                inGameUI.gameObject.SetActive(false);
                paused = true;
                Time.timeScale = 0f;
            }
            
        }        
    }


}

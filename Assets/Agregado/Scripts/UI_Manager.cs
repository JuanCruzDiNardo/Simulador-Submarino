using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public Canvas pauseUI;    
    public Canvas inGameUI;
    public Canvas TutorialUI;    
    public Canvas GalleryUI;
    public Canvas Summary;

    public TextMeshProUGUI txtFishes;
    public TextMeshProUGUI txtTotalPoints;

    public int TutorialStep = -1;

    public bool paused = false;
    public bool inSummary = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseUI.gameObject.SetActive(false);
        GalleryUI.gameObject.SetActive(false);
        Summary.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        TutorialUI.gameObject.SetActive(true);
        paused = false;               
    }

    // Update is called once per frame
    void Update()
    {
        pause();
        StepCheck();
        CloseSummary();
    }

    private void CloseSummary()
    {
        if (inSummary && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Return)))
        {
            ShowSummary();
        }
            
    }

    public void ShowSummary()
    {
        Time.timeScale = Time.timeScale == 0f? 1f : 0f;

        inSummary = !inSummary;
        inGameUI.gameObject.SetActive(!inSummary);
        Summary.gameObject.SetActive(inSummary);

        CreateFishList();
    }

    private void CreateFishList()
    {
        txtFishes.text = string.Empty;        

        List<fish> fishList = Screenshot_Controller.fishCaught;

        foreach (species fishSpecies in Enum.GetValues(typeof(species)))
        {
            int count = fishList.Where(x => x.FishSpecies == fishSpecies).Count();
            int points = fishList.Where(x => x.FishSpecies == fishSpecies).Sum(S => S.FishPoints);

            if (count > 0)
                txtFishes.text += $"{fishSpecies} x{count}  {points}P. \n";
        }

        txtTotalPoints.text = $"Total Points: {Screenshot_Controller.totalPoints}";
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

        for (int i = 0; i < TutorialUI.transform.childCount; i++)
        {
            if (i == TutorialStep)
            {
                TutorialUI.transform.GetChild(i).gameObject.SetActive(true);
            }else
            {
                TutorialUI.transform.GetChild(i).gameObject.SetActive(false);
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
        paused = !paused;
        pauseUI.gameObject.SetActive(paused);
        TutorialUI.gameObject.SetActive(!paused);
        GalleryUI.gameObject.SetActive(paused);
        inGameUI.gameObject.SetActive(!paused);

        Time.timeScale = 1f;
    }

    // Función para ir al menú principal
    public void LoadMainMenu()
    {
        /*
        Time.timeScale = 1f;           // Restablecer el tiempo (en caso de estar pausado)
        SceneManager.LoadScene("MainMenu"); // Cargar la escena del menú principal
        */
    }

    // Función para salir del juego
    public void QuitGame()
    {
        Debug.Log("Quit Game!");       // Esto se verá en el editor
        Application.Quit();            // Cerrar el juego (solo funciona en la build)
    }

    public void OpenGallery()
    {
        pauseUI.gameObject.SetActive(false);
        GalleryUI.gameObject.SetActive(true);
        GalleryUI.GetComponent<Image_Manager>().LoadGallery();
    }

    public void CloseGallery()
    {
        pauseUI.gameObject.SetActive(true);
        GalleryUI.gameObject.SetActive(false);
    }

    private void pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inSummary)
        {
            if (paused)
            {
                paused = !paused;
                pauseUI.gameObject.SetActive(paused);
                TutorialUI.gameObject.SetActive(!paused);
                GalleryUI.gameObject.SetActive(paused);
                inGameUI.gameObject.SetActive(!paused);
                
                Time.timeScale = 1f;
            }
            else
            {
                paused = !paused;
                pauseUI.gameObject.SetActive(paused);
                TutorialUI.gameObject.SetActive(!paused);
                inGameUI.gameObject.SetActive(!paused);
                
                Time.timeScale = 0f;
            }
            
        }        
    }


}

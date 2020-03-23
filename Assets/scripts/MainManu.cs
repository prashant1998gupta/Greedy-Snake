using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManu : MonoBehaviour
{
    
    
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

   
    public void PlayGameAferGameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
    public void GoTOMainManu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);

    }


    public void Quit()
    {
        Debug.Log("game is close");
         
        Application.Quit();
    }
}

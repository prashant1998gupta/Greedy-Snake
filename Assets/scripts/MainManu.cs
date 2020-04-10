using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManu : MonoBehaviour
{

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play(SoundType.BUTTONCLICK);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }


    public void PlayGameAferGameOver()
    {
        AudioManager.instance.Play(SoundType.BUTTONCLICK);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
    public void GoTOMainManu()
    {
        AudioManager.instance.Play(SoundType.BUTTONCLICK);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);

    }


    public void Quit()
    {
        Debug.Log("game is close");
        AudioManager.instance.Play(SoundType.BUTTONCLICK);
        Application.Quit();
    }
}

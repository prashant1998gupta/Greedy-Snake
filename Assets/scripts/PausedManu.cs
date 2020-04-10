using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedManu : MonoBehaviour
{

    public GameObject Pause_manu, PauseButton;

    public GameManager gameManager;
    

    public void Pause()
    {
        AudioManager.instance.Play(SoundType.BUTTONCLICK);
        if (gameManager.isGameOver)
            return;
        Pause_manu.SetActive(true);
        PauseButton.SetActive(false);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        AudioManager.instance.Play(SoundType.BUTTONCLICK);
        Pause_manu.SetActive(false);
        PauseButton.SetActive(true);
        Time.timeScale = 1;
    }



}

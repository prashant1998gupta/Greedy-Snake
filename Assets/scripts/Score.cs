using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public TMPro.TextMeshProUGUI HighScoreText1;
    public TMPro.TextMeshProUGUI currentScoreText1;



    void Start()
    {
        HighScoreText1.text = PlayerPrefs.GetInt("highScoreText").ToString();
        currentScoreText1.text = PlayerPrefs.GetInt("CurrentScore").ToString();

    }



}
   

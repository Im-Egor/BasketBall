using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text scoreText;  
    private int score = 0;  

    private void OnTriggerEnter(Collider other)
    {
        UpdateScore(1);  
    }

    private void UpdateScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }
}


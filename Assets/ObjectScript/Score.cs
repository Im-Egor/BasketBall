using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text _scoreText;  
    private int _score = 0;  

    private void OnTriggerEnter(Collider other)
    {
        UpdateScore(1);  
    }

    private void UpdateScore(int points)
    {
        _score += points;
        _scoreText.text = "Score: " + _score;
    }
}


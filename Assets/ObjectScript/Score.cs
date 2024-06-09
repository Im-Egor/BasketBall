using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] Text _scoreText;  // UI элемент для отображения очков
    private int _score = 0;  // Переменная для хранения очков

    private void OnTriggerEnter(Collider other)
    {
        UpdateScore(1);  // Обновляем очки при попадании
    }

    private void UpdateScore(int points)
    {
        _score += points;
        _scoreText.text = "Score: " + _score;
    }
}


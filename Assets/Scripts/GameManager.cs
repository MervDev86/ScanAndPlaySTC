using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour {

    int score;
    public static GameManager instance;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Player playerMovement;

    public void IncrementScore ()
    {
        score++;
        scoreText.text = score.ToString();
        // Increase the player's speed
        playerMovement.speed += playerMovement.speedIncreasePerPoint;
    }

    private void Awake ()
    {
        instance = this;
    }

    private void Start () {

	}

	private void Update () {
	
	}
}
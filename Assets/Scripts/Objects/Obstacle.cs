using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour {

    PlayerGameHandler _mPlayerGameHandler;

	private void Start () {
        _mPlayerGameHandler = GameObject.FindObjectOfType<PlayerGameHandler>();
	}

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            _mPlayerGameHandler.Hit();
            Destroy(gameObject);
        }
    }

}
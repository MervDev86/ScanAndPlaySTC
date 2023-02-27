using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour {

    Player m_player;

	private void Start () {
        m_player = GameObject.FindObjectOfType<Player>();
	}

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            m_player.Hit();
            Destroy(gameObject);
        }
    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Coin : MonoBehaviour {

    public int points = 10;
    [SerializeField] float m_turnSpeed = 90f;

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null) {
            Destroy(gameObject);
            return;
        }

        // Check that the object we collided with is the player
        if (other.gameObject.tag != "Player") {
            return;
        }

        Destroy(gameObject);
    }

    private void Start () {

	}

	private void Update () {
        transform.Rotate(0, 0, m_turnSpeed * Time.deltaTime);
	}
}
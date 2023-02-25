using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    float horizontalInput;
    bool alive = true;
    public float speed = 5;
    public float speedIncreasePerPoint = 0.1f;
    [SerializeField] float horizontalMultiplier = 2;

    private void FixedUpdate()
    {
        if (!alive) return;

        //Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        //Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime * horizontalMultiplier;
        //rb.MovePosition(rb.position + forwardMove + horizontalMove);

        transform.Translate(transform.forward * speed * Time.fixedDeltaTime);
    }

}

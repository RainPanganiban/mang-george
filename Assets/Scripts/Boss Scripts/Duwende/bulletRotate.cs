using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletRotate : MonoBehaviour
{
    private Renderer bulletRenderer;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.MoveRotation(rb.rotation + 200 * Time.fixedDeltaTime);
    }
}

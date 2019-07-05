using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Vector3 speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = new Vector3(Random.Range(.3f, .6f), Random.Range(.3f, .6f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed);
    }
}

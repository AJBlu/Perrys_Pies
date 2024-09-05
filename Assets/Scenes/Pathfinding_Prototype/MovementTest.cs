using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    bool moveRight = false;

    // Update is called once per frame
    void Update()
    {
        if (!moveRight)
        {
            if (transform.position.x < 10)
            {
                transform.position += new Vector3(10 * Time.deltaTime, 0, 0);
            }
            else
            {
                moveRight = true;
            }
        }
        else
        {
            if(transform.position.x > -10)
            {
                transform.position += new Vector3(-10 * Time.deltaTime, 0, 0);
            }
            else
            {
                moveRight = false;
            }
        }
    }
}

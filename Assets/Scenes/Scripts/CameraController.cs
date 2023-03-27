using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float speed = 100f;
    float rotationAroundX = 45;
    float rotationAroundY = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(rotationAroundX, rotationAroundY, 0);
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationAroundY += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationAroundY -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow) && rotationAroundX < 90)
        {
            rotationAroundX += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) && rotationAroundX > 0)
        {
            rotationAroundX -= speed * Time.deltaTime;
        }
    }
}

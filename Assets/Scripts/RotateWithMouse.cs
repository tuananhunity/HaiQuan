using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
     float speed = 10;

    bool isRotate = false;

    Vector3 mouse;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotate = true;
            mouse = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotate = false;
        }

        if (isRotate)
        {
            Vector3 deltaMouse = Input.mousePosition - mouse;

            float rotationX = deltaMouse.y * speed * Time.deltaTime;
            float rotationY = -deltaMouse.x * speed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotationY, Space.World);
            transform.Rotate(Vector3.right, rotationX, Space.World);

            mouse = Input.mousePosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScaleWithScrollWheel : MonoBehaviour
{
    public float speed = 100f;
    public float min = 0.1f;
    public float max = 5;

  

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        Vector3 scaleChange = transform.localScale * (1 + scrollInput * speed);

        scaleChange = new Vector3(
            Mathf.Clamp(scaleChange.x, min, max),
            Mathf.Clamp(scaleChange.y, min, max),
            Mathf.Clamp(scaleChange.z, min, max));

        transform.localScale = scaleChange;
    }
}
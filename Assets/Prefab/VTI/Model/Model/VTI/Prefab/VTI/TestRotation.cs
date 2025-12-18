using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool flip = false;
    [SerializeField] private bool lockPos = false;

    private Vector3 _init;
    
    private void Awake()
    {
        _init = transform.position;
    }

    private void Update()
    {
        var flipMultiplier = flip ? -1 : 1;
        // rotate z axis over time smoothly
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.eulerAngles += new Vector3(0, 0, -rotationSpeed * flipMultiplier * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.eulerAngles += new Vector3(0, 0, rotationSpeed * flipMultiplier * Time.deltaTime);
        }

        if (!lockPos) return; 
        transform.position = _init;
    }
}

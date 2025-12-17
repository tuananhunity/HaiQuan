using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public BoPhanMay boPhanMay;
    
    Controller controller;
    public void OnButtonClick()
    {
        if(!controller)
            controller = FindObjectOfType<Controller>();

        controller.OnButtonClick(this);
        
    }
}

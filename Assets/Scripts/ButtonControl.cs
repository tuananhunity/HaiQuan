using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonControl : MonoBehaviour
{
    public Button Button;
    public BoPhanMay boPhanMay;
    public ButtonType buttonType;

    public Image image;
    public Transform toggleTransform;
    private bool isOn = false;
    public bool IsOn
    {
        get { return isOn; }
        set 
        {
            isOn = value;
            image.DOKill();
            toggleTransform.DOKill();
            if (isOn)
            {
                if(image != null)
                {
                    image.DOFade(1f, 0.5f);
                }
                else
                {
                    toggleTransform.DORotate(new Vector3(0f, 0f, -90f), 0.5f);
                }
            }
            else
            {
                if(image != null)
                {
                    image.DOFade(0.5f, 0.5f);
                }
                else
                {
                    toggleTransform.DORotate(Vector3.zero, 0.5f);
                }
            }
        }       
    }

}

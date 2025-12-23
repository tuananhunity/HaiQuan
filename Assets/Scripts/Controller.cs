using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public List<ButtonControl> buttonControls;
    public List<DicBoPhanMayBottle> dicBoPhanMayBottles;

    void Awake()
    {
        foreach (var buttonControl in buttonControls)
        {
            buttonControl.Button.onClick.AddListener(() => OnButtonClick(buttonControl));
        }
    }

    void OnDestroy()
    {
        foreach (var buttonControl in buttonControls)
        {
            buttonControl.Button.onClick.RemoveAllListeners();
        }
    }

    public void OnButtonClick(ButtonControl buttonControl)
    {
        Debug.Log("Button clicked: " + buttonControl.buttonType + " for " + buttonControl.boPhanMay);
        BoPhanMay boPhanMay = buttonControl.boPhanMay;
        ButtonType buttonType = buttonControl.buttonType;

        switch (buttonType)
        {
            case ButtonType.SwitchLocalRemote:
                buttonControl.IsOn = !buttonControl.IsOn;
                break;
            case ButtonType.LockOpen:
                buttonControl.IsOn = !buttonControl.IsOn;
                break;
            case ButtonType.StartLocal:
                if (!IsControlRemote(boPhanMay))
                {
                    buttonControl.IsOn = true;
                    StartRelease(buttonControl);
                    EnableStartButtons(boPhanMay);
                }
                break;
            case ButtonType.StopLocal:
                if (!IsControlRemote(boPhanMay))
                {
                    buttonControl.IsOn = true;
                    StopRelease(buttonControl);
                    DisableStartButtons(boPhanMay);
                }
                break;
            case ButtonType.StartRemote:
                if (IsControlRemote(boPhanMay))
                {
                    Debug.Log("StartRemote button clicked for " + boPhanMay);
                    buttonControl.IsOn = !buttonControl.IsOn;
                    if(buttonControl.IsOn)
                        StartRelease(buttonControl);
                    else
                        StopRelease(buttonControl);
                }
                break;
            default:
                Debug.LogWarning("Unhandled ButtonType: " + buttonControl.buttonType);
                break;
        }
    }

    public bool IsControlRemote(BoPhanMay boPhanMay)
    {
        foreach (var buttonControl in buttonControls)
        {
            if (buttonControl.boPhanMay == boPhanMay && buttonControl.buttonType == ButtonType.SwitchLocalRemote)
            {
                return buttonControl.IsOn;
            }
        }
        return false;
    }

    public void DisableStartButtons(BoPhanMay boPhanMay)
    {
        foreach (var buttonControl in buttonControls)
        {
            if (buttonControl.boPhanMay == boPhanMay && 
                (buttonControl.buttonType == ButtonType.StartLocal || buttonControl.buttonType == ButtonType.StartRemote))
            {
                buttonControl.IsOn = false;
            }
        }
    }

    public void EnableStartButtons(BoPhanMay boPhanMay)
    {
        foreach (var buttonControl in buttonControls)
        {
            if (buttonControl.boPhanMay == boPhanMay && 
                (buttonControl.buttonType == ButtonType.StartLocal || buttonControl.buttonType == ButtonType.StartRemote))
            {
                buttonControl.IsOn = true;
            }
            else if(buttonControl.boPhanMay == boPhanMay && 
                (buttonControl.buttonType == ButtonType.StopLocal))
            {
                buttonControl.IsOn = false;
            }
        }
    }

    public void StartRelease(ButtonControl buttonControl)
    {
        Debug.Log("Start Release called for " + buttonControl.boPhanMay);
        if(buttonControl.buttonType == ButtonType.StartRemote)
        {
            foreach (var dic in dicBoPhanMayBottles)
            {
                if (dic.BoPhanMay == buttonControl.boPhanMay)
                {
                    foreach (var bottle in dic.Bottles)
                    {
                        Debug.Log("Start Release Remote Animation for " + dic.BoPhanMay);
                        bottle.StartReleaseRemoteAnimation();
                    }
                }
            }
        }
    }

    public void StopRelease(ButtonControl buttonControl)
    {
        Debug.Log("Stop Release called for " + buttonControl.boPhanMay);
        if(buttonControl.buttonType == ButtonType.StartRemote)
        {
            foreach (var dic in dicBoPhanMayBottles)
            {
                if (dic.BoPhanMay == buttonControl.boPhanMay)
                {
                    foreach (var bottle in dic.Bottles)
                    {
                        bottle.StopReleaseRemoteAnimation();
                    }
                }
            }
        }
    }
}

public enum BoPhanMay
{
    TrungTamNganh5,
    KhoangMayTruoc,
    KhoangMaySau,
    BangDieuKhienChinh2,
    KhoangSecto,
}

public enum ButtonType
{
    SwitchLocalRemote,
    LockOpen,
    StartLocal,
    StopLocal,
    StartRemote
}

[System.Serializable]
public class DicBoPhanMayBottle
{
    public BoPhanMay BoPhanMay;
    public List<Bottle> Bottles;
}

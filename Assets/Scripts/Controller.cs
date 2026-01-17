using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    public static Controller Instance;
    public List<ButtonControl> buttonControls;
    public List<DicBoPhanMayBottle> dicBoPhanMayBottles;
 
    public List<VideoClip> lstVideo;
    public VideoPlayer player;
    public GameObject mode;
    public GasFlowPathEffect fxKhiHopChay;
    public VideoPlayer videoHopKhi;
    public GameObject lockPanel;
    public bool isLockPanel = true;
    public GameObject namefd;
    
    private int count;
    public void Play(int index)
    {
        count++;
        player.gameObject.SetActive(true);
        mode.SetActive(false);
        player.clip = lstVideo[index];
        player.Play();
        namefd.SetActive(false);
        lockPanel.SetActive(false);
        DOVirtual.DelayedCall((float)lstVideo[index].length, () =>
        {
            namefd.SetActive(true);
            lockPanel.SetActive(true);
            player.gameObject.SetActive(false);
            mode.SetActive(true);
            if(count == 4)
            {
                lockPanel.SetActive(false);
                isLockPanel = false;
            }
        });
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        buttonControls = new List<ButtonControl>(FindObjectsOfType<ButtonControl>());
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
        if (isLockPanel)
        {
            
        }
        Debug.Log("Button clicked: " + buttonControl.buttonType + " for " + buttonControl.boPhanMay);
        BoPhanMay boPhanMay = buttonControl.boPhanMay;
        ButtonType buttonType = buttonControl.buttonType;

        switch (buttonType)
        {
            case ButtonType.SwitchLocalRemote:
                buttonControl.IsOn = !buttonControl.IsOn;
                SwitchLocalRemote(buttonControl);
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

    Tween abc;
    Tween def;
    Tween ongXa;
    public GasFlowPathEffect gas;
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
                        videoHopKhi.Play();
                        fxKhiHopChay.StartFlow();
                        bottle.StartReleaseRemoteAnimation();

                        abc?.Kill();
                        def?.Kill();

                        dic.denSanSang.DOFade(1f, 0f);
                        dic.denDangXa.DOFade(1f, 0f);
                        
                        abc = DOVirtual.DelayedCall(2f, () =>
                        {
                            dic.denSanSang.DOFade(0.5f, 0);
                        });
                        def = DOVirtual.DelayedCall(30f, () =>
                        {
                            dic.denDangXa.DOFade(0.5f, 0);
                        });
                        ongXa?.Kill();
                        ongXa = DOVirtual.DelayedCall(25, () =>
                        {
                            gas.StartFlow();
                        });
                    }
                }
            }
        }

        if(buttonControl.buttonType == ButtonType.StartLocal)
        {
            foreach (var dic in dicBoPhanMayBottles)
            {
                if (dic.BoPhanMay == buttonControl.boPhanMay)
                {
                    foreach (var bottle in dic.Bottles)
                    {
                        bottle.StartReleaseRemoteAnimation();
                        ongXa?.Kill();
                        ongXa = DOVirtual.DelayedCall(25, () =>
                        {
                            gas.StartFlow();
                        });
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
                        fxKhiHopChay.StopFlow();
                        videoHopKhi.Stop();
                        videoHopKhi.frame = 0;
                        bottle.StopReleaseRemoteAnimation();

                        abc?.Kill();
                        def?.Kill();

                        dic.denSanSang.DOFade(0.5f, 0f);
                        dic.denDangXa.DOFade(0.5f, 0f);
                        ongXa?.Kill();
                        gas.StopFlow();
                    }
                }
            }
        }

        if(buttonControl.buttonType == ButtonType.StopLocal)
        {
            foreach (var dic in dicBoPhanMayBottles)
            {
                if (dic.BoPhanMay == buttonControl.boPhanMay)
                {
                    foreach (var bottle in dic.Bottles)
                    {
                        ongXa?.Kill();
                        gas.StopFlow();
                        videoHopKhi.Stop();
                        videoHopKhi.frame = 0;
                        fxKhiHopChay.StopFlow();
                        bottle.StopReleaseRemoteAnimation();
                    }
                }
            }
        }
    }

    public void SwitchLocalRemote(ButtonControl buttonControl)
    {
        foreach (var dic in dicBoPhanMayBottles)
        {
            if (dic.BoPhanMay == buttonControl.boPhanMay)
            {
                if(buttonControl.IsOn)
                    dic.denTuXa.DOFade(1f, 0.5f);
                else
                    dic.denTuXa.DOFade(0.5f, 0.5f);
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
    public Image denTuXa;
    public Image denDangXa;
    public Image denSanSang;
    public BoPhanMay BoPhanMay;
    public List<Bottle> Bottles;
}

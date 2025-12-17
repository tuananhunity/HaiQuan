using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTaiLieuThamKhao : MonoBehaviour
{
    public GameObject contentTaiLieuThietKeSwuf76;
    public GameObject contentTaiLieuKhac;

    private bool isOpenOption;


    public void TaiLieuThietKeSwuf76()
    {
        InActiveAll();

        isOpenOption = true;

        contentTaiLieuThietKeSwuf76.SetActive(true);
    } 
    public void TaiLieuKhac()
    {
        InActiveAll();

        isOpenOption = true;

        contentTaiLieuKhac.SetActive(true);
    }

    public void OnBack()
    {
        if (isOpenOption)
        {
            InActiveAll();
        }
        else
        {
            MainPanel.Instance.OnBackHome();
        }
    }

    public void InActiveAll()
    {
        isOpenOption = false;

        contentTaiLieuThietKeSwuf76.SetActive(false);
        contentTaiLieuKhac.SetActive(false);
    }
}

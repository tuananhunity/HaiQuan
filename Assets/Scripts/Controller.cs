using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public void OnButtonClick(ButtonControl buttonControl)
    {
        BoPhanMay boPhanMay = buttonControl.boPhanMay;
        switch (boPhanMay)
        {
            case BoPhanMay.TrungTamNganh5:
                // Handle TrungTamNganh5 button click
                break;
            case BoPhanMay.KhoangMayTruoc:
                // Handle KhoangMayTruoc button click
                break;
            case BoPhanMay.KhoangMaySau:
                // Handle KhoangMaySau button click
                break;
            case BoPhanMay.BangDieuKhienChinh2:
                // Handle BangDieuKhienChinh2 button click
                break;
            case BoPhanMay.KhoangSecto:
                // Handle KhoangSecto button click
                break;
            default:
                Debug.LogWarning("Unhandled BoPhanMay: " + boPhanMay);
                break;
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickChuanBi : MonoBehaviour
{
    public ChuanBi chuanBi;
    public int ID;
    // Start is called before the first frame update
    void OnMouseDown()
    {
        Debug.Log("Bam ChuanBi: " + chuanBi);
        if (DicCheckChuanBi[chuanBi])
        {
            return;
        }

        DicCheckChuanBi[chuanBi] = true;
        Controller.Instance.Play(ID);
    }

    public enum ChuanBi
    {
        VanChan,
        VanDauBinh,
        VanKichHoat,
        VanVTI
    }

    public static Dictionary<ChuanBi, bool> DicCheckChuanBi = new Dictionary<ChuanBi, bool>
    {
        {ChuanBi.VanChan, false},
        {ChuanBi.VanDauBinh, false},
        {ChuanBi.VanKichHoat, false},
        {ChuanBi.VanVTI, false}
    };
}

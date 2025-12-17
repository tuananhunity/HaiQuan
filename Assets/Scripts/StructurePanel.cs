using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePanel : MonoBehaviour
{
    public GameObject panelModel3D;
    public PanelMoPhongDongCo panelMoPhongDongCo;
    public GameObject contentCauTaoHopGiamVong;
    public List<GameObject> lstModel3D;

    private bool isOpenOption;

    public void OnOpenModel3D()
    {
        InActiveAll();

        isOpenOption = true;
        panelModel3D.SetActive(true);
    }

    public void OnOpenPanelMoPhongDongCo()
    {
        InActiveAll();

        isOpenOption = true;
        panelMoPhongDongCo.gameObject.SetActive(true);
    } 
    public void OnOpenCauTaoHopGiamVong()
    {
        InActiveAll();

        isOpenOption = true;
        contentCauTaoHopGiamVong.SetActive(true);
    }

    public void OnChooseModel(GameObject go)
    {
        int index = int.Parse(go.name);

        lstModel3D.ForEach(model => model.SetActive(false));
        lstModel3D[index].SetActive(true);
    }

    public void Back()
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
        panelMoPhongDongCo.InActiveAll();

        panelModel3D.SetActive(false);
        panelMoPhongDongCo.gameObject.SetActive(false);
        contentCauTaoHopGiamVong.SetActive(false);
        lstModel3D.ForEach(model => model.SetActive(false));
    }
}

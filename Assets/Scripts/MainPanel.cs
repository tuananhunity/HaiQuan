using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    public static MainPanel Instance;

    [SerializeField] private string pathProcess = "C:/Users/84866/Documents/Document/ToiletGame/demo.mp4";

    [SerializeField] private GameObject panelMain;
    [SerializeField] private PanelMoDau panelMoDau;
    [SerializeField] private StructurePanel panelCauTao;
    [SerializeField] private PanelTaiLieuThamKhao panelTaiLieuThamKhao;
    [SerializeField] private PanelTaiLieuThamKhao panelCauTao1;
    [SerializeField] private GameObject goBaoQuan;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OpenPanelMoDau()
    {
        panelMain.SetActive(false);
        panelMoDau.gameObject.SetActive(true);
    }

    public void OpenPanelCauTao()
    {
        
        panelMain.SetActive(false);
        panelCauTao.gameObject.SetActive(true);
    }

    public void OpenPanelTaiLieuThamKhao()
    {
        panelMain.SetActive(false);
        goBaoQuan.SetActive(true);
    }

    public void OnBackHome()
    {
        panelMain.SetActive(true);

        panelCauTao1.InActiveAll();
        panelMoDau.InActiveAll();
        panelCauTao.InActiveAll();
        panelTaiLieuThamKhao.InActiveAll();


        panelMoDau.gameObject.SetActive(false);
        panelCauTao.gameObject.SetActive(false);
        panelTaiLieuThamKhao.gameObject.SetActive(false);
        panelCauTao1.gameObject.SetActive(false);
        goBaoQuan.SetActive(false);
    }

    public void OpenProcessPanelControl()
    {
        panelMain.SetActive(false);
        panelCauTao1.gameObject.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTaiLieuThamKhao : MonoBehaviour
{
    public List<ModelConfig> lst;

    private bool isOpenOption;

    public void OpenThe(int index)
    {
        InActiveAll();
        lst[index].model.SetActive(true);
        lst[index].name.SetActive(true);
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
        lst.ForEach(item =>
        {
            item.model.SetActive(false);
            item.name.SetActive(false);
        });
    }

[Serializable]
    public class ModelConfig
    {
        public GameObject name;
        public GameObject model;
    }
}

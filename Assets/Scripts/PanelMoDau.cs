using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMoDau : MonoBehaviour
{
    public GameObject contentThuyetMinh;
    public List<GameObject> lstContentScroll;
    public List<Button> lstButtonContent;

    private bool isOpenOption;

    private void Start()
    {
        InActiveContent();

        lstButtonContent.ForEach(button =>
        {
            button.onClick.AddListener(()=> OnClickButtonContent(button));
        });
    }

    private void OnClickButtonContent(Button button)
    {
        InActiveContent();

        int index = int.Parse(button.name);
        lstContentScroll[index].SetActive(true);
    }

    public void OnThuyetMinh()
    {
        InActiveAll();

        isOpenOption = true;

        contentThuyetMinh.SetActive(true);
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

        contentThuyetMinh.SetActive(false);
    }

    private void InActiveContent()
    {
        lstContentScroll.ForEach(content => content.SetActive(false));
    }
}

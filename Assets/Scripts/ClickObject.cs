using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    private Bottle bottle;

    private Bottle Bottle
    {
        get
        {
            if (bottle == null)
            {
                bottle = GetComponent<Bottle>();
            }
            return bottle;
        }
    }
    void OnMouseDown()
    {
        Bottle.StartSimple();
    }
}

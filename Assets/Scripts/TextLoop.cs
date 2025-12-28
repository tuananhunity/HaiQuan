using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TextLoop : MonoBehaviour
{
    public Transform a;

    public Transform pointA;
    public Transform pointB;
    public float duration;

    private void Start()
    {
        Loop();
    }

    void Loop()
    {
        a.transform.position = pointA.position;
        a.DOMove(pointB.position, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Loop();
        });
    }
}

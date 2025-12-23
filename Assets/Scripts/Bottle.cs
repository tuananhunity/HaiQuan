using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Animator animator;
    public GasFlowPathEffect gasFlowEffectKhiday;
    public GasFlowPathEffect gasFlowEffect;

    public float timeKhiDay = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void StopReleaseRemoteAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        gasFlowEffect.StopFlow();
    }

    public void StartReleaseRemoteAnimation()
    {
        gasFlowEffectKhiday.StartFlow();
        DOVirtual.DelayedCall(timeKhiDay, () =>
        {
            animator.SetTrigger("Open");
            gasFlowEffect.StartFlow();
        }); 
        
    }
}

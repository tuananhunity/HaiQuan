using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Animator animator;
    public GasFlowPathEffect gasFlowEffectKhiday;
    public GasFlowPathEffect gasFlowEffect;
    public List<Bottle> bottleDependency;

    public float timeKhiDay = 2f;
    public bool isProcess;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void StopReleaseRemoteAnimation()
    {
        isProcess = false;
        if (abc != null)
        {
            abc.Kill();
        }
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        gasFlowEffectKhiday?.StopFlow();
        gasFlowEffect?.StopFlow();
        foreach (var bottleDependency in bottleDependency)
        {
            bottleDependency.StopReleaseRemoteAnimation();
        }
    }

    Tween abc;
    public void StartReleaseRemoteAnimation()
    {
        isProcess = true;
        if(abc != null)
        {
            abc.Kill();
        }
        gasFlowEffectKhiday?.StartFlow();

        abc = DOVirtual.DelayedCall(timeKhiDay, () =>
        {
            animator.SetTrigger("Open");
            gasFlowEffect.StartFlow();
            foreach (var bottleDependency in bottleDependency)
            {
                bottleDependency.StartReleaseRemoteAnimation();
            }   
        }); 
    }

    public void StartSimple()
    {
        if(Controller.Instance.isLockPanel)
            return;
        if (isProcess)
        {
            return;
        }
        
        animator.SetTrigger("Open");
        gasFlowEffect.StartFlow();
    }
}

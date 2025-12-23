using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Animator animator;
    public GasFlowPathEffect gasFlowEffect;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void StopReleaseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        gasFlowEffect.StopFlow();
    }

    public void StartReleaseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
        gasFlowEffect.StartFlow();
    }
}

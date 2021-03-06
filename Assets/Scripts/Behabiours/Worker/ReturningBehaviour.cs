﻿using System;
using UnityEngine;

public class ReturningBehaviour : StateMachineBehaviour
{
    private int collectedGold;
    private GameObject base_home;
    
    private PathWalker pathWalker;
    private TextMesh behaviourDisplay;
    private MineDetector mineDetector;

    override public async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (pathWalker == null)
                pathWalker = animator.gameObject.GetComponent<PathWalker>();
            if (behaviourDisplay == null)
                behaviourDisplay = animator.gameObject.GetComponentInChildren<TextMesh>();
            if (base_home == null)
                base_home = GameManager.Instance.GetBase();
            if (mineDetector == null)
                mineDetector = animator.gameObject.GetComponentInChildren<MineDetector>();

            behaviourDisplay.text = $"{ GetType() }";
            collectedGold = animator.GetInteger("Gold");
            base_home = GameManager.Instance.GetBase();

            SoundManager.Instance.PlaySound(Sound.Peasant_Yes_My_Lord_Sound_Effect);
            await pathWalker.WalkTo(base_home.transform.position);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (pathWalker.Ended)
            {
                GameManager.Instance.AddGold(collectedGold);
                animator.SetInteger("Gold", 0);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mineDetector.ResetDetector();
        animator.SetBool("MinesInView", false);
    }
}

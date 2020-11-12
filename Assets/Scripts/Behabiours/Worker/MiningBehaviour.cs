using System;
using UnityEngine;

public class MiningBehaviour : StateMachineBehaviour
{
    private int collectedGold;

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
            if (mineDetector == null)
                mineDetector = animator.gameObject.GetComponentInChildren<MineDetector>();

            if (mineDetector.Mine == null)
            {
                animator.SetBool("MinesInView", false);
                return;
            }

            await pathWalker.WalkTo(mineDetector.Mine.Position);
            SoundManager.Instance.PlaySound(Sound.Peasant_More_Work_Sound_Effect);
            behaviourDisplay.text = $"{ GetType() }";
            collectedGold = animator.GetInteger("Gold");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    float mineTime = 0.25f;
    float mineCurrentTime = 0f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (mineDetector.Mine == null && collectedGold < 10)
        {
            animator.SetBool("MinesInView", false);
            return;
        }
        if (pathWalker.Ended)
        {
            mineCurrentTime += Time.deltaTime;
            if (mineCurrentTime < mineTime)
                return;

            if (mineDetector.Mine != null && mineDetector.Mine.CanMine() && collectedGold < 10)
            {
                mineCurrentTime = 0f;
                collectedGold += mineDetector.Mine.CollectGold(1);
                return;
            }
            animator.SetInteger("Gold", collectedGold);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mineDetector.ResetDetector();
    }
}

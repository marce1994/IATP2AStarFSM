using System;
using UnityEngine;

public class MarkingBehaviour : StateMachineBehaviour
{
    private MineDetector explorerDetector;
    private PathWalker pathWalker;
    private TextMesh behaviourDisplay;

    override public async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (pathWalker == null)
                pathWalker = animator.gameObject.GetComponent<PathWalker>();
            if (behaviourDisplay == null)
                behaviourDisplay = animator.gameObject.GetComponentInChildren<TextMesh>();
            if (explorerDetector == null)
                explorerDetector = animator.gameObject.GetComponentInChildren<MineDetector>();

            behaviourDisplay.text = $"{ GetType() }";

            SoundManager.Instance.PlaySound(Sound.Footman_As_You_Wish_Sound_Effect);
            
            await pathWalker.WalkTo(explorerDetector.Mine.Position);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!pathWalker.Ended) return;
        animator.SetBool("MinesInView", false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(explorerDetector.Mine != null && animator.gameObject.transform.position == explorerDetector.Mine.Position)
            explorerDetector.Mine.Flagged = true;
        
        explorerDetector.ResetDetector();
    }
}

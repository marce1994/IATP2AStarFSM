using System;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
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

            behaviourDisplay.text = $"{ GetType() }";
            await pathWalker.WalkTo(PathFinderManager.Instance.RandomWalkablePosition);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    override public async void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (mineDetector.Collided)
            {
                animator.SetBool("MinesInView", true);
                return;
            }
            if (pathWalker.Ended)
                await pathWalker.WalkTo(PathFinderManager.Instance.RandomWalkablePosition);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}

using System;
using System.Linq;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    private PathWalker pathWalker;
    private TextMesh behaviourDisplay;
    private MineDetector mineDetector;
    private Creature creature;

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
            if (creature == null)
                creature = animator.gameObject.GetComponent<Creature>();

            behaviourDisplay.text = $"{ GetType() }";
            await pathWalker.WalkTo(PathFinderManager.Instance.RandomWalkablePosition);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            OnStateEnter(animator, stateInfo, layerIndex);
        }
    }

    override public async void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (GameManager.Instance.Creatures.Where(x => x.name.Contains("Enemy")).Any(x => (x.transform.position - animator.transform.position).magnitude < 10f))
            {
                animator.SetBool("EnemyInRange", true);
                if(animator.gameObject.name.Contains("Explorer"))
                    return;
            }
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

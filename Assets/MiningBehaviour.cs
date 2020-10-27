using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiningBehaviour : StateMachineBehaviour
{
    int collectedGold;
    Mine mine;
    Transform transform;
    Vector3[] path;

    int current;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        transform = animator.transform;
        mine = GameManager.Instance.GetRandomFlaggedMine();
        PathFinderManager.Instance.FindPath(transform.position, mine.Position).ContinueWith(result => {
            if (!result.IsFaulted)
            {
                path = result.Result;
                current = 0;
            }
            else
            {
                Debug.LogWarning(result.Exception.Message);
            }
        });
        collectedGold = 0;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (path == null)
            return;

        if (transform.position == path.Last())
        {
            if (mine.CanMine())
                collectedGold += mine.CollectGold(Mathf.Max(0,10 - collectedGold));
            
            if (collectedGold != 10)
            {
                path = null;
                mine = GameManager.Instance.GetRandomFlaggedMine();
                PathFinderManager.Instance.FindPath(transform.position, mine.Position).ContinueWith(result => {
                    if (!result.IsFaulted)
                    {
                        path = result.Result;
                        current = 0;
                    }
                    else
                    {
                        Debug.LogWarning(result.Exception.Message);
                    }
                });
            }

            animator.SetInteger("Gold", collectedGold);

            return;
        }

        if (transform.position != path[current])
        {
            if (current == 0)
                transform.position = path[current];

            Vector3 pos = Vector3.MoveTowards(transform.position, path[current], Time.deltaTime);
            transform.position = pos;
        }
        else current = (current + 1) % path.Length;
    }
}

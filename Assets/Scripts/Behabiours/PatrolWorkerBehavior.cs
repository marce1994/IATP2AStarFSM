using System.Linq;
using UnityEngine;

public class PatrolWorkerBehavior : StateMachineBehaviour
{
    private Transform transform;
    private Vector3[] path;

    private int current;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<TextMesh>().text = this.GetType().ToString();

        transform = animator.transform;

        PathFinderManager.Instance.FindPath(transform.position, PathFinderManager.Instance.RandomWalkablePosition).ContinueWith(result =>
        {
            if (!result.IsFaulted)
            {
                if (result.Result == null) return;

                path = result.Result;
                current = 0;
            }
            else
            {
                Debug.LogWarning(result.Exception.Message);
            }
        });
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (path == null || transform.position == path.Last())
        {
            PathFinderManager.Instance.FindPath(transform.position, PathFinderManager.Instance.RandomWalkablePosition).ContinueWith(result =>
            {
                if (!result.IsFaulted)
                {
                    if (result.Result == null) return;

                    path = result.Result;
                    current = 0;
                }
                else
                {
                    Debug.LogWarning(result.Exception.Message);
                }
            });
            return;
        }

        if (transform.position != path[current])
        {
            if (current == 0)
                transform.position = path[current];

            Vector3 pos = Vector3.MoveTowards(transform.position, path[current], Time.deltaTime * 10);
            transform.position = pos;
        }
        else current = (current + 1) % path.Length;
    }
}

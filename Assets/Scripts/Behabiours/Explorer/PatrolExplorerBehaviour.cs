using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolExplorerBehaviour : StateMachineBehaviour
{
    private Transform transform;
    private Vector3[] path;

    private int current;
    private ExplorerDetector _explorerDetector;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<TextMesh>().text = this.GetType().ToString();
        _explorerDetector = animator.gameObject.GetComponentInChildren<ExplorerDetector>();

        transform = animator.transform;

        PathFinderManager.Instance.FindPath(transform.position, PathFinderManager.Instance.RandomWalkablePosition).ContinueWith(result =>
        {
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

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_explorerDetector.CollidedMine != null)
        {
            animator.SetBool("MinesInView", true);
            return;
        }

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
            float angle = 0;

            Vector3 relative = _explorerDetector.gameObject.transform.InverseTransformPoint(path[current]);
            angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg - 90;
            _explorerDetector.gameObject.transform.Rotate(0, 0, -angle);

            if (current == 0)
                transform.position = path[current];

            Vector3 pos = Vector3.MoveTowards(transform.position, path[current], Time.deltaTime * 10);
            transform.position = pos;
        }
        else current = (current + 1) % path.Length;
    }
}

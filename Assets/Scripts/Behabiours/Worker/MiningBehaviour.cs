using System.Linq;
using UnityEngine;

public class MiningBehaviour : StateMachineBehaviour
{
    private int collectedGold;
    private Mine mine;
    private Transform transform;
    private Vector3[] path;

    private int current;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<TextMesh>().text = this.GetType().ToString();

        transform = animator.transform;
        mine = GameManager.Instance.GetRandomFlaggedMine();

        if (mine == null)
            return;

        SoundManager.Instance.PlaySound(Sound.Peasant_More_Work_Sound_Effect);
        PathFinderManager.Instance.FindPath(transform.position, mine.Position).ContinueWith(result =>
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
        collectedGold = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (path == null)
        {
            OnStateEnter(animator, stateInfo, layerIndex);
            return;
        }

        if (transform.position == path.Last())
        {
            if (mine != null && mine.CanMine())
                collectedGold += mine.CollectGold(Mathf.Max(0, 10 - collectedGold));

            if (collectedGold < 10)
            {
                path = null;
                mine = GameManager.Instance.GetRandomFlaggedMine();
                if (mine == null) return;

                PathFinderManager.Instance.FindPath(transform.position, mine.Position).ContinueWith(result =>
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

            animator.SetInteger("Gold", collectedGold);

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

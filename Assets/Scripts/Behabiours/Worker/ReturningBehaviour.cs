using System.Linq;
using UnityEngine;

public class ReturningBehaviour : StateMachineBehaviour
{
    private int collectedGold;
    private GameObject base_home;
    private Transform transform;
    private Vector3[] path;

    private int current;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<TextMesh>().text = this.GetType().ToString();

        collectedGold = animator.GetInteger("Gold");
        transform = animator.transform;
        base_home = GameManager.Instance.GetBase();

        if (base_home == null)
            return;

        SoundManager.Instance.PlaySound(Sound.Peasant_Yes_My_Lord_Sound_Effect);
        PathFinderManager.Instance.FindPath(transform.position, base_home.transform.position).ContinueWith(result =>
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
        if (transform.position == path.Last())
        {
            GameManager.Instance.AddGold(collectedGold);
            animator.SetInteger("Gold", 0);
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

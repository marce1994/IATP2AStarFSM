using UnityEngine;

public class MarkingBehaviour : StateMachineBehaviour
{
    private ExplorerDetector _explorerDetector;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<TextMesh>().text = this.GetType().ToString();
        _explorerDetector = animator.gameObject.GetComponentInChildren<ExplorerDetector>();

        if (_explorerDetector.CollidedMine.Flagged)
        {
            animator.SetBool("MinesInView", false);
            _explorerDetector.ResetDetector();
            return;
        }

        _explorerDetector.CollidedMine.Flagged = true;
        SoundManager.Instance.PlaySound(Sound.Footman_As_You_Wish_Sound_Effect);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_explorerDetector.CollidedMine == null)
        {
            _explorerDetector.ResetDetector();
            animator.SetBool("MinesInView", false);
            return;
        }
    }
}

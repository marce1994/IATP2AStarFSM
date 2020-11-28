using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    Creature enemy;
    Creature creature;
    PathWalker pathWalker;
    float timer = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (pathWalker == null)
                pathWalker = animator.gameObject.GetComponent<PathWalker>();
            if (creature == null)
                creature = animator.gameObject.GetComponent<Creature>();

            var go = GameManager.Instance.Creatures.Where(x => x.name.Contains("Enemy")).FirstOrDefault(x => (x.gameObject.transform.position - animator.gameObject.transform.position).magnitude < creature.view_range);

            if (go == null)
            {
                animator.SetBool("EnemyInRange", false);
                return;
            }

            SoundManager.Instance.PlaySound(Sound.Footman_Aggro);
            enemy = go.GetComponent<Creature>();
            await pathWalker.WalkTo(enemy.gameObject.transform.position + ((animator.transform.position - enemy.gameObject.transform.position).normalized * 1.5f));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public async void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            if (pathWalker.Ended)
            {
                if ((enemy.transform.position - animator.transform.position).magnitude > creature.attack_range)
                {
                    await pathWalker.WalkTo(enemy.transform.position + ((animator.transform.position - enemy.transform.position).normalized * 1.5f));
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer > 1f)
                    {
                        SoundManager.Instance.PlaySound(Sound.Footman_Attack);
                        enemy.RecibeDamage(animator.gameObject.GetComponent<Creature>().Attack);
                        
                        if (!enemy.Alive)
                        {
                            enemy = null;
                            animator.SetBool("EnemyInRange", false);
                        }

                        timer = 0;
                    }
                }
        }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            animator.SetBool("EnemyInRange", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = null;
        timer = 1;
    }
}

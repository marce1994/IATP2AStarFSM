using BehaviorTree;
using System.Linq;
using UnityEngine;

public class EnemyManager : Creature
{
    BehaviorComponent _root;
    PathWalker PathWalker;
    Transform enemyTransform;

    private void Awake()
    {
        base.Awake();
        PathWalker = gameObject.GetComponent<PathWalker>();
        
        var _builder = new BehaviourTreeBuilder();

        _builder = _builder.Selector(); // MainNode
        
        _builder = _builder.StateFulSequence("Patrol")
                        .Inverter("!").Condition("HayEnemigo", () => enemyTransform != null).End()
                        .Condition("EndPath", () => PathWalker.Ended)
                        .Do("FindPath", () => { _ = PathWalker.WalkTo(PathFinderManager.Instance.RandomWalkablePosition); PathWalker.WALK_SPEED = 5; return BehaviorReturnCode.Success; })
                    .End();

        _builder = _builder.StateFulSequence("FindEnemy")
                         .Inverter("!").Condition("HayEnemigo", () => enemyTransform != null).End()
                         .Inverter("!").Do("BuscarEnemigo", FindEnemy).End()
                    .End();

        _builder = _builder.StateFulSequence("Follow")
                        .Condition("HayEnemigo", () => enemyTransform != null)
                        .Inverter("!").Condition("EstaCerca", () => (enemyTransform.position - transform.position).magnitude < attack_range).End()
                        .Condition("EndPath", () => PathWalker.Ended)
                        .Do("FindPath",() => { _ = PathWalker.WalkTo(enemyTransform.position + ((transform.position - enemyTransform.position).normalized * 1.5f)); return BehaviorReturnCode.Success; })
                    .End();

        _builder = _builder.StateFulSequence("Attack")
                        .Condition("HayEnemigo", () => enemyTransform != null)
                        .Condition("EstaCerca", () => (enemyTransform.position - transform.position).magnitude < attack_range)
                        .Do("AttackEnemy", AttackEnemy)
                    .End();

        _builder = _builder.End(); // End MainNode

        _root = _builder.Build();
    }

    float timeCounter = 0f;
    BehaviorReturnCode AttackEnemy()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter < attack_speed)
            return BehaviorReturnCode.Running;
        timeCounter = 0f;
        
        Debug.Log("Atacando enemigo");
        var enemy = enemyTransform.gameObject.GetComponent<Creature>();
        
        if (enemy == null || !enemy.Alive)
        {
            enemyTransform = null;
            return BehaviorReturnCode.Failure;
        }

        SoundManager.Instance.PlaySound(Sound.Wolf_Attack);
        enemy.RecibeDamage(Attack);
        return BehaviorReturnCode.Success;
    }

    BehaviorReturnCode FindEnemy()
    {
        Debug.Log("Buscando enemigo");

        var go = GameManager.Instance.Creatures
        .Where(x => x.name.Contains("Explorer") || x.name.Contains("Worker"))
        .Where(x => x.GetComponent<Creature>() != null)
        .FirstOrDefault(x => (x.gameObject.transform.position - transform.position).magnitude < view_range);

        if (go == null)
            return BehaviorReturnCode.Failure;

        enemyTransform = go.transform;
        Debug.Log("Encontrado enemigo");
        _ = PathWalker.WalkTo(enemyTransform.position); // Corta el camino anterior
        PathWalker.WALK_SPEED = 12;
        SoundManager.Instance.PlaySound(Sound.Wolf_Aggro);
        return BehaviorReturnCode.Success;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        _root.Behave();
    }
}

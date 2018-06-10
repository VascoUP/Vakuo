using UnityEngine;

public class EnemyScriptEnabler : ItemsEnabler
{
    private EnemyController[] _enemyControllers;
    private AttackPlayer[] _enemyAttacks;

    private void Start()
    {
        if (_enemyControllers == null)
            GetItems();
    }

    public void GetItems()
    {
        _enemyControllers = GetComponentsInChildren<EnemyController>();
        _enemyAttacks = GetComponentsInChildren<AttackPlayer>();
    }

    public override void EnableItems(Transform target, bool isEnable)
    {
        if(!isEnable)
            return;
        if (_enemyControllers == null)
            GetItems();
        target = isEnable ? target : null;
        foreach(EnemyController controller in _enemyControllers)
        {
            if(controller != null) {
                controller.SetTarget(target);
                controller.enabled = isEnable;
            }
        }
    }
}

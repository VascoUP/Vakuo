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

    public override void EnableItems(bool isEnable)
    {
        if (_enemyControllers == null)
            GetItems();
        foreach(EnemyController controller in _enemyControllers)
        {
            if(controller != null)
                controller.enabled = isEnable;
        }
        foreach (AttackPlayer attack in _enemyAttacks)
        {
            if (attack != null)
                attack.enabled = isEnable;
        }
    }
}

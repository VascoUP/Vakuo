public class EnemyScriptEnabler : ItemsEnabler
{
    private EnemyController[] _enemyControllers;

    private void Start()
    {
        if (_enemyControllers == null)
            GetItems();
    }

    public void GetItems()
    {
        _enemyControllers = GetComponentsInChildren<EnemyController>();
        foreach(EnemyController controller in _enemyControllers)
        {

        }
    }

    /*public void OnEnemyDeath(GameObject enemy)
    {

    }*/

    public override void EnableItems(bool isEnable)
    {
        if (_enemyControllers == null)
            GetItems();
        foreach(EnemyController controller in _enemyControllers)
        {
            controller.enabled = isEnable;
        }
    }
}

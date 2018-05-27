using UnityEngine;

public abstract class ScriptActivator : MonoBehaviour {

    public bool isActive = false;

    protected abstract void Run();

    public void SetActive(bool active)
    {
        isActive = active;
    }

    // Update is called once per frame
    private void Update()
    {
        if(isActive)
        {
            Run();
        }
    }
}

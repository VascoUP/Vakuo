using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunDisintegrate : MonoBehaviour {

    public List<Material> materials;

    [SerializeField]
    private float _disintegrateTime = 2f;
    private float _value = 0f;

    public bool enabledScript = false;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void Enable(bool enable)
    {
        enabledScript = enable;
        if(enabledScript)
        {
            _value = 0f;
        }
    }

    private void SetProperty(string name, object value)
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetFloat(name, (float)value);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void DisintegrateStep()
    {
        if (_value > 1f)
            return;

        _value += Time.deltaTime / _disintegrateTime;
        foreach (Material mat in materials)
        {
            SetProperty("_SliceAmount", _value > 1f ? 1f: _value);
            //mat.SetFloat("_SliceAmount", _value);
        }
    }

	private void Update () {
        if(enabledScript)
        {
            DisintegrateStep();
        }
	}
}

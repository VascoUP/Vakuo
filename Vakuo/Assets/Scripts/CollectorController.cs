using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectorController : MonoBehaviour {
    [SerializeField]
    private Text _coinText;

    private int counter = 0;

    public void Collect()
    {
        counter++;
        _coinText.text = "Coinss: " + counter;
    }
}

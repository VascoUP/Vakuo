using UnityEngine;
using UnityEngine.UI;


public class Intro : MonoBehaviour {
    public float textRate;

    public TextAsset file;

    public GameObject textBackground;

    [Range(0,1)]
    public float backgroundOpacity;

    private int line = 0;

    private int cursor = 0;

    private float nextTime = 0;

    private string[] lines;

    private bool endOfLine = false;

    //private bool stopAnimation = false;

    void Start() {
        lines = file.text.Split('\n');

        SpriteRenderer renderer = textBackground.GetComponent<SpriteRenderer>();
        Color tmp = renderer.color;
        tmp.a = backgroundOpacity;
        renderer.color = tmp;
	}

	void Update () {
        Text label = gameObject.GetComponent<Text>();
        if (line < lines.Length && !endOfLine)
        {
            if (label.text != lines[line])
            {
                if (Time.time >= nextTime)
                {
                    label.text += lines[line][cursor];
                    cursor++;
                    nextTime = Time.time + textRate;
                }
            }
            else
            {
                endOfLine = true;
            }
        } else

            if (endOfLine)
        {
            line++;
            cursor = 0;
            label.text = "";
            endOfLine = false;
        }
    }
}

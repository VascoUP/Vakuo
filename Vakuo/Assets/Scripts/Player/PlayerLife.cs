using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour {
	public GameObject panel;
    public GameObject heartPrefab;
	public float lifes;

	private List<Image> heartsImages;
    private float initialLifes;

	private void OnEnable () {
        this.heartsImages = new List<Image>();
        CreateLifePanel();
        initialLifes = lifes;
	}

    void CreateLifePanel(){
        for (int i = 0; i < this.lifes; i++)
        {
            AddImage(i);
        }
    }

	void AddImage(int index){
        float x = 60 * index;

        Transform transform = panel.transform;
		GameObject instance = Instantiate(heartPrefab, panel.transform);
		RectTransform rect = instance.GetComponent<RectTransform>();
        rect.Translate(new Vector3(x, 0, 0));
		Image myImage = instance.GetComponent<Image>();
		this.heartsImages.Add(myImage);
	}

	float RemoveLifes() {
        this.lifes -= 0.25f;

        if (this.lifes >= 0)
        {
            if (Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
            {
                this.heartsImages[this.heartsImages.Count - 1].fillAmount = 0;
                this.heartsImages.RemoveAt(this.heartsImages.Count - 1);
            }
            else
            {
                if (this.heartsImages[this.heartsImages.Count - 1].fillAmount > 0)
                {
                    this.heartsImages[this.heartsImages.Count - 1].fillAmount -= 0.25f;
                }
            }
        }
        return this.lifes; //Para depois ver se o player morre ou não
	}

    void AddLifes(float count)
    {
        while (this.lifes < count)
        {
			AddImage((int)this.lifes);
            this.lifes++;
        }
    }

    public void Damage()
    {
        float livesRemaining = RemoveLifes();
        if (livesRemaining <= 0)
        {
            AstronautController controller = gameObject.GetComponent<AstronautController>();
            controller.Respawn();
            AddLifes(initialLifes);
        }
    }
}

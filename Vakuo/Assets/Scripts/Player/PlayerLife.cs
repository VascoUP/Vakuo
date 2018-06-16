using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour {
	public GameObject panel;
  public GameObject heartPrefab;
	public float lifes;

	private List<Image>heartsImages;

	private void OnEnable () {
        this.heartsImages = new List<Image>();
        CreateLifePanel();
	}

    void CreateLifePanel(){
        for (int i = 0; i < this.lifes; i++)
        	AddImage(i);
    }

	void AddImage(int index){
		float x  = -350 + 125 * index;
		float y = 180;

		GameObject instance = Instantiate(heartPrefab, panel.transform);
		RectTransform rect = instance.GetComponent<RectTransform>();
		rect.localPosition = new Vector3(x, y, 0f);
		Image myImage = instance.GetComponent<Image>();
		this.heartsImages.Add(myImage);
	}

	float RemoveLifes() {
        this.lifes -= 0.25f;

        if(this.lifes > 0){
            if (Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
                this.heartsImages.RemoveAt(this.heartsImages.Count - 1);
            else
                if (this.heartsImages[this.heartsImages.Count - 1].fillAmount > 0)
                    this.heartsImages[this.heartsImages.Count - 1].fillAmount -= 0.25f;
        }
        return this.lifes; //Para depois ver se o player morre ou não
	}

    void AddLifes(){
        if (this.lifes <= 4){
            this.lifes++;
						int index = this.heartsImages.Count - 1;
						AddImage(index);
						this.heartsImages[index].fillAmount = (this.lifes + 1f) - this.heartsImages.Count;
        }
				else{
					this.lifes = 5;
					this.heartsImages[this.heartsImages.Count - 1].fillAmount = 1f;
				}
    }
}

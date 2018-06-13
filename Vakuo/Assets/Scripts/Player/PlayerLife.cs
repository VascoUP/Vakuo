using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour {
	public GameObject panel;
    public GameObject heartPrefab;
	public double lifes;

	private List<Image>heartsImages;

	private void OnEnable () {
        this.heartsImages = new List<Image>();
        CreateLifePanel();
	}

    void CreateLifePanel(){
        float x = 100;
        float y = 100;
        float deltaX = 200;
        for (int i = 0; i < this.lifes; i++)
        {
            Debug.Log("New heart");
            GameObject instance = Instantiate(heartPrefab, panel.transform);
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(x, y, 0f);
            x += deltaX;
            Image myImage = instance.GetComponent<Image>();
            this.heartsImages.Add(myImage);

        }
    }
	
	void RemoveLifes() {
        this.lifes -= 0.25;

        if (Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
            this.heartsImages.RemoveAt(this.heartsImages.Count - 1);
        else{
            if (this.heartsImages[this.heartsImages.Count - 1].fillAmount > 0)
                this.heartsImages[this.heartsImages.Count - 1].fillAmount -= 0.25f;
                         
        }
        
	}

    void Addlifes(){
        if (this.lifes < 5){
            if (Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
                this.lifes++;
            else{
                this.heartsImages[this.heartsImages.Count - 1].fillAmount = 1.0f;
                this.lifes = this.heartsImages.Count;
            }
        }
    }
}

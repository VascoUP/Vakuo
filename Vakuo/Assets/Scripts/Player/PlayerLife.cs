using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public GameObject panel;
    public GameObject heartPrefab;
    private float maxLifes;
	public float lifes;

	private List<Image> heartsImages;

	private void OnEnable ()
    {
        this.heartsImages = new List<Image>();
        CreateLifePanel();
        maxLifes = lifes;
	}

    void CreateLifePanel(){
        for (int i = 0; i < this.lifes; i++)
        {
            AddImage(i);
        }
    }

	void AddImage(int index){
        float x = (60 * index) + 10;

        Transform transform = panel.transform;
		GameObject instance = Instantiate(heartPrefab, panel.transform);
		RectTransform rect = instance.GetComponent<RectTransform>();
        rect.Translate(new Vector3(x, -20, 0));
		Image myImage = instance.GetComponent<Image>();
		this.heartsImages.Add(myImage);
	}

	float RemoveLifes() {
        if (this.lifes <= 0)
            return this.lifes;

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

    void RemoveAllLifes()
    {
        while (this.lifes >= 0)
        {
            RemoveLifes();
        }
    }

    void AddLifes(float count)
    {
        while (this.lifes < count)
        {
            AddImage((int)this.lifes);
            this.lifes++;
        }
    }

    public void AddLife()
    {
        if (this.lifes <= maxLifes - 1)
        {
            int index = this.heartsImages.Count - 1;
            this.heartsImages[index].fillAmount = 1f;
            this.lifes++;
            index++;
            AddImage(index);
            Debug.Log("ADDED IMAGE AT " + index + " lifes " + this.lifes + " " + ((this.lifes + 1f) - this.heartsImages.Count));
            this.heartsImages[index].fillAmount = (this.lifes + 1f) - this.heartsImages.Count;
        }
        else
        {
            this.lifes = maxLifes;
            this.heartsImages[this.heartsImages.Count - 1].fillAmount = 1f;
        }
    }

    public void Kill()
    {
        foreach(Image image in this.heartsImages)
        {
            Destroy(image);
        }

        this.lifes = 0f;
        this.heartsImages.Clear();
        AddLifes(maxLifes);
    }

    public void Damage()
    {
        float livesRemaining = RemoveLifes();
        if (livesRemaining <= 0)
        {
            Respawn();
        }
    }

    public void Damage(int multiplier)
    {
        for (int i = 0; i < multiplier; ++i)
        {
            float livesRemaining = RemoveLifes();
            if (livesRemaining <= 0)
            {
                Respawn();
                break;
            }
        }
    }

    private void Respawn()
    {
        AstronautController controller = gameObject.GetComponent<AstronautController>();
        controller.Respawn();
        AddLifes(maxLifes);
    }
}

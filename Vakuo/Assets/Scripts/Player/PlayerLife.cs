using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [SerializeField]
    private GameObject _hurtTrigger;

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
        Debug.Log("End create panel");
    }

	void AddImage(int index){
        float x = (60 * index) + 10;

        Debug.Log(x + "->" + index);

        Transform transform = panel.transform;
		GameObject instance = Instantiate(heartPrefab, panel.transform);
		RectTransform rect = instance.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(x, -20, 0);
		Image myImage = instance.GetComponent<Image>();
		this.heartsImages.Add(myImage);
        Debug.Log(rect.position + "->" + index);
	}

	float RemoveLifes() {
        if (this.lifes <= 0)
            return this.lifes;

        this.lifes -= 0.25f;

        if (this.lifes >= 0)
        {
            if (Math.Abs(this.lifes % 1) <= (Double.Epsilon * 100))
            {
                Destroy(this.heartsImages[this.heartsImages.Count - 1].gameObject);
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
        _hurtTrigger.SetActive(true);
        _hurtTrigger.SetActive(false);
        foreach(Image image in this.heartsImages)
        {
            Destroy(image.gameObject);
        }

        this.lifes = 0f;
        this.heartsImages.Clear();
        AddLifes(maxLifes);
    }

    public void Damage()
    {
        _hurtTrigger.SetActive(true);
        _hurtTrigger.SetActive(false);
        float livesRemaining = RemoveLifes();
        if (livesRemaining <= 0)
        {
            Respawn();
        }
    }

    public void Damage(int multiplier)
    {
        _hurtTrigger.SetActive(true);
        _hurtTrigger.SetActive(false);
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

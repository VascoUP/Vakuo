using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using System.Collections.Generic;

public class EntityInventory : MonoBehaviour
{
    public BasicContainer mainContainer = new BasicContainer();

    void AddAnimalsToContainer(List<int>animals)
    {
        string animalName = "";
        List<string>animalsNames = new List<string>();
        foreach (int animal in animals)
        {
            switch (animal)
            {
                case 0:
                    animalName = "Elephant";
                    break;
                case 1:
                    animalName = "Turtle";
                    break;
                default:
                    animalName = "Question";
                    break;
            }

            animalsNames.Add(animalName);
        }

        //OnGUI(animalsNames);
    }

    void OnGUI(/*List<string>animals*/)
    {
        for(int i=0; i < mainContainer.slotAmount; i++) {
            BasicItem item = null;
            if (i == 0)
                item = new BasicItem("Elephant", "Elephant");
            else if (i == 1)
                item = new BasicItem("Turtle", "Turtle");
            else
                item = new BasicItem("Blocked", "Question");
   
            mainContainer.AddItemAsASingleStack(item);
        }

        /*foreach(string name in animals)
        {
            BasicItem item = new BasicItem(name, name);
            mainContainer.AddItemAsASingleStack(item);
        }*/


    }
    private void Start()
    {
        mainContainer.OnInventoryChange.AddListener(delegate { Debug.Log("Inventory changed!"); });
    }

    // Update is called once per frame
    void Update()
    {

    }

}
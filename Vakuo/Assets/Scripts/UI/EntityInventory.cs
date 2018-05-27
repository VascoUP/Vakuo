using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using System.Collections.Generic;

public class EntityInventory : MonoBehaviour
{
    public BasicContainer mainContainer = new BasicContainer();

    private void Start()
    {
        mainContainer.OnInventoryChange.AddListener(delegate { });
    }

    private void OnGUI()
    {
        // TODO: Load animals from save file
    }

    public void AddAnimal(BasicItem item)
    {
        mainContainer.AddItemAsASingleStack(item);
    }

    private void AddAnimalsToContainer(List<int>animals)
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
}
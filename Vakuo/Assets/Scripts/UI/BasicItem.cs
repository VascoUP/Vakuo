using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    public class BasicItem
    {
        #region properties
        private string name; // the name of the item
        public UnityEvent OnNameChange = new UnityEvent(); // the event invoked when the property changes
        public string Name // getters and setters of the property which trigger the event
        {
            get { return name; }
            set { name = value; OnNameChange.Invoke(); }
        }
        private string graphic; // a string that holds the name of the graphical representation of the item: Sprite in the UI or the model in game-world
        public UnityEvent OnGraphicChange = new UnityEvent();
        public string Graphic
        {
            get { return graphic; }
            set { graphic = value; OnGraphicChange.Invoke(); }
        }
        #endregion

        #region constructors 

        // Basic constructor without parameters
        public BasicItem()
        {
            Name = "Unnamed Item";
            Graphic = "Unknown";
        }
        // Main constructor with basic parameters
        public BasicItem(string itemName, string itemGraphic)
        {
            Name = itemName;
            Graphic = itemGraphic;
        }
        #endregion
    }
}

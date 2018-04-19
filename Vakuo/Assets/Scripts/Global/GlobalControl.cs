using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {

    public delegate void SaveDelegate(object sender, EventArgs args);
    public static event SaveDelegate SaveEvent;

    public delegate void LoadDelegate(object sender, EventArgs args);
    public static event LoadDelegate LoadEvent;

    static GlobalControl instance;

    public static GlobalControl Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GlobalControl>();
                if(instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<GlobalControl>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
        SaveEvent(null, null);
    }

    public void Load()
    {
        LoadEvent(null, null);
    }
}

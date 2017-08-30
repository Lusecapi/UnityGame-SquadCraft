using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class editWorldMenuScript : MonoBehaviour
{
    UIController uiController;
    private string worldFile;
    //private RootMenu parentScript;
    private bool isPressedBack;

    #region Getters and Setters

    //public RootMenu ParentScript
    //{
    //    get { return parentScript; }
    //    set { parentScript = value; }
    //}

    public string WorldFile
    {
        get
        {
            return worldFile;
        }

        set
        {
            worldFile = value;
        }
    }

    #endregion

    void Start()
    {
        uiController = GameObject.FindGameObjectWithTag("UI Controller").GetComponent<UIController>();
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = this.gameObject;
        isPressedBack = false;
        //parentScript.SwitchEnable();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isPressedBack)
            {
                OnCloseButtonClick();
            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            isPressedBack = false;
        }

    }

    public void OnCloseButtonClick()
    {
        uiController.activeMenu = uiController.beforeMenu;
        uiController.beforeMenu = null;
        Destroy(this.gameObject);
        //parentScript.PrefabDestroy(this.gameObject);
    }

    public void OnDeleteWorldButtonClick()
    {
        MapEditor.deleteWorld(worldFile);
        //parentScript.PrefabDestroy(this.gameObject);
        uiController.activeMenu = uiController.beforeMenu;
        uiController.beforeMenu = null;
        Destroy(this.gameObject);
    }

}

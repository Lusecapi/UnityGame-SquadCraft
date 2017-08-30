using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour
{


    [Header("Menu Panels")]
    [SerializeField]
    GameObject mainMenuPanel;
    [SerializeField]
    GameObject selectWorldPanel;
    [SerializeField]
    GameObject settingsPanel;
    [SerializeField]
    GameObject exitPanel;
    [Header("Settings Menu Elementt")]
    [SerializeField]
    InputField nameField;
    [SerializeField]
    Toggle maleToogle;
    [SerializeField]
    Toggle femaleToogle;

    //Private variables
    private UIController uiController;
	private bool isPressedBack;
    //public bool isActiveUIController;


    private List<WorldButton> worldButtonsList;
    private int lastWorldButtonIndex;//The index of the last world button, to easy access

    // Use this for initialization
    void Start()
    {
        uiController = GetComponent<UIController>();
        selectWorldPanel.SetActive(false);
        settingsPanel.SetActive(false);
        exitPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        uiController.activeMenu = mainMenuPanel;
        //isActiveUIController = true;
        isPressedBack = false;
        if (!SettingsManager.WasDataLoaded)
        {
            SettingsManager.loadSettings();
            SettingsManager.WasDataLoaded = true;
        }

        //Settings panel
        nameField.text = SettingsManager.UserName;
        if (SettingsManager.UserGender.Equals(SettingsManager.Gender.Female))
        {
            femaleToogle.isOn = true;
        }
        else
        {
            maleToogle.isOn = true;
        }

        lastWorldButtonIndex = -1;
        worldButtonsList = new List<WorldButton>();
    }

    void Update ()
	{
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isPressedBack)
            {
                isPressedBack = true;
                if (uiController.activeMenu == mainMenuPanel)
                {
                    On_ExitButton_Click();
                }
                else
                    if (uiController.activeMenu == selectWorldPanel || uiController.activeMenu == settingsPanel)
                {
                    On_BackToMenuButton_Click();
                }
                else
                    if(uiController.activeMenu == exitPanel)
                {
                    On_NoExitButton_Click();
                }
            }
        }
        if (!Input.GetButtonDown("Cancel"))
        {
            isPressedBack = false;
        }
    }

    #region Funtions and Methods

    #endregion

    #region UI Methods
    public void On_NewGameButton_Click()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        selectWorldPanel.SetActive(true);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = selectWorldPanel;

        WorldButton.scrollViewContent = GameObject.Find("SceneUICanvas/PlayMenuPanel/MyWorldsScrollView/Viewport/Content").GetComponent<RectTransform>();
        updateWorldButtons();
        //isActiveUIController = false;
    }

    public void On_ExitButton_Click ()
	{
        //Message.showConfirmationMessage(0, ConfirmationMessageAction.QuitApplication);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = exitPanel;
        exitPanel.SetActive(true);

	}

	public void On_SettingsButton_Click ()
	{
        mainMenuPanel.SetActive(false);
        selectWorldPanel.SetActive(false);
        settingsPanel.SetActive(true);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = settingsPanel;
        //isActiveUIController = false;
	}

	public void On_MapEditButton_Click ()
	{
		SceneManager.LoadScene (2);
	}

    public void On_BackToMenuButton_Click()
    {
        selectWorldPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = mainMenuPanel;
        //if(uiController.activeMenu == selectWorldPanel)
        //{
        //    worldButtonsList.Clear();
        //}
        //isActiveUIController = true;
    }

    public void On_ApplySettingsButton_Click()
    {
        nameField.text = nameField.text.Trim();
        if (nameField.text != "")
        {
            SettingsManager.IsMale = maleToogle.isOn;
            SettingsManager.UserName = nameField.text;
            if (SettingsManager.IsMale)
            {
                SettingsManager.UserGender = SettingsManager.Gender.Male;
            }
            else
            {
                SettingsManager.UserGender = SettingsManager.Gender.Female;
            }
            Debug.Log(SettingsManager.UserGender);
            Debug.Log(SettingsManager.UserName);
            SettingsManager.saveSettings();
            //parentScript.PrefabDestroy(this.gameObject);
        }
        else
        {
            Debug.Log("Bad_Name");
            Message.showMessageText("Please write a valid Name", MessageType.Error);
            Text name = nameField.gameObject.GetComponentInParent<Text>();
            name.fontStyle = FontStyle.Italic;
        }
    }

    public void OnYesExitButton_Click()
    {
        Application.Quit();
    }

    public void On_NoExitButton_Click()
    {
        exitPanel.SetActive(false);
        uiController.activeMenu = uiController.beforeMenu;
        uiController.activeMenu = mainMenuPanel;
    }

    #endregion


    /// <summary>
    /// Update or reload world buttons. It's use when there is a new world created
    /// </summary>
    private void updateWorldButtons()
    {
        if (Directory.Exists((MapEditor.WorldsFilesPath)))
        {
            destroyWorldsButtons();
            try
            {
                string[] worldsPaths = Directory.GetFiles(MapEditor.WorldsFilesPath);
                for (int i = 0; i < worldsPaths.Length; i++)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    string[] pathArray = worldsPaths[i].Split('/');//on android he path are "/Folder1/Folder2/myworld.txt
#elif UNITY_EDITOR
                    string[] pathArray = worldsPaths[i].Split('\\'); //on pc the path is like /Folder1/Folder2\\myworld.txt
#endif
                    string worldFile = pathArray[pathArray.Length - 1];
                    createWorldButton(worldFile);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Message.showMessageText(e.ToString(),20);
            }
            //updateWorlds = false;
        }
    }

    /// <summary>
    /// To create a world button.
    /// </summary>
    /// <param name="wordlFile"> receives the world file name (string).</param>
    private void createWorldButton(string wordlFile)
    {
        if (worldButtonsList.Count != 0)
        {
            worldButtonsList.Add(new WorldButton(wordlFile, new Vector2(worldButtonsList[lastWorldButtonIndex].AnchoredPos.x, worldButtonsList[lastWorldButtonIndex].AnchoredPos.y - 65)));
        }
        else
        {
            worldButtonsList.Add(new WorldButton(wordlFile));
        }
        lastWorldButtonIndex++;
        setContentHeight();
    }


    private void setContentHeight()
    {
        WorldButton.scrollViewContent.sizeDelta = new Vector2(0, Mathf.Abs(worldButtonsList[lastWorldButtonIndex].ButtonRectTransform.offsetMin.y));//the offsetMin.y, returns something like the heigh of the worldbutton gameobject
    }

    /// <summary>
    /// To destroy all the world buttons and clear the world button list
    /// </summary>
    private void destroyWorldsButtons()
    {
        if (worldButtonsList.Count > 0)
        {
            for (int i = 0; i < worldButtonsList.Count; i++)
            {
                Destroy(worldButtonsList[i].WorldButtonGameObject);
            }
            lastWorldButtonIndex = -1;
            worldButtonsList.Clear();
            WorldButton.scrollViewContent.sizeDelta = new Vector2(0, 0);
            //worldButtonsList = new List<WorldButton>();
        }
    }



    /// <summary>
    /// Class to help manage worldButtons
    /// </summary>
    private class WorldButton
    {
        #region Default Values
        private GameObject worldButtonPrefab = Resources.Load("Menus/WorldButton") as GameObject;
        private Vector2 defaultAnchoredPos = new Vector2(0f, -30f);
        private Vector2 defaultOffsetMin = new Vector2(0f, -60f);
        private Vector2 defaultOffsetMax = new Vector2(0f, 0f);
        private Vector3 defaultScale = Vector3.one;
        #endregion
        //Actual Button parameters
        public static RectTransform scrollViewContent;
        private GameObject worldButtonGameObject;
        private Vector2 anchoredPos;
        private RectTransform buttonRectTransform;
        private string worldFile;
        private string worldName;

        #region Getters and Setter

        public GameObject WorldButtonGameObject
        {
            get
            {
                return worldButtonGameObject;
            }

            set
            {
                worldButtonGameObject = value;
            }
        }

        public Vector2 AnchoredPos
        {
            get
            {
                return anchoredPos;
            }

            set
            {
                anchoredPos = value;
            }
        }

        public RectTransform ButtonRectTransform
        {
            get
            {
                return buttonRectTransform;
            }

            set
            {
                buttonRectTransform = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor, to create a new World Button
        /// </summary>
        /// <param name="theWorldFile">The File</param>
        /// <param name="theAnchoredPos">The position</param>
        /// <param name="theRootMenu">The root menu, always use "this"</param>
        public WorldButton(string theWorldFile, Vector2 theAnchoredPos)
        {
            worldButtonGameObject = Instantiate(worldButtonPrefab);
            anchoredPos = theAnchoredPos;
            buttonRectTransform = WorldButtonGameObject.GetComponent<RectTransform>();
            worldFile = theWorldFile;
            worldName = worldFile.Split('.')[0];
            setButtonProperties();

        }

        /// <summary>
        /// Constructor to create a World Button
        /// </summary>
        /// <param name="theWorldFile">The File</param>
        /// <param name="theRootMenu">The r0ot menu, always use "this"</param>
        public WorldButton(string theWorldFile)
        {
            worldButtonGameObject = Instantiate(worldButtonPrefab);
            anchoredPos = defaultAnchoredPos;
            buttonRectTransform = WorldButtonGameObject.GetComponent<RectTransform>();
            worldFile = theWorldFile;
            worldName = worldFile.Split('.')[0];
            setButtonProperties();
        }

        /// <summary>
        /// To set all the button properties
        /// </summary>
        /// <param name="theRootMenu">The root menu</param>
        private void setButtonProperties()
        {
            worldButtonGameObject.GetComponent<worldButtonScript>().WorldFile = worldFile;
            //worldButtonGameObject.GetComponent<worldButtonScript>().ParentScript = theRootMenu;
            worldButtonGameObject.GetComponent<worldButtonScript>().WorldType = WorldGenerator.WorldType.MyWorld;
            worldButtonGameObject.GetComponent<RectTransform>().SetParent(scrollViewContent);
            worldButtonGameObject.GetComponent<RectTransform>().offsetMin = defaultOffsetMin;
            worldButtonGameObject.GetComponent<RectTransform>().offsetMax = defaultOffsetMax;
            worldButtonGameObject.GetComponent<RectTransform>().localScale = defaultScale;
            worldButtonGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            setButtonText();
        }

        /// <summary>
        /// To set the World Button text or tittle we wnat to show
        /// </summary>
        private void setButtonText()
        {
            worldButtonGameObject.GetComponentInChildren<Text>().text = worldName;
        }
    }

}

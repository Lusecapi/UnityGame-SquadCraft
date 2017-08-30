using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class gameUIScript : MonoBehaviour
{
    UIController uiController;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject exitGameConfirmationMenu;
    [SerializeField]
    GameObject quickInventoryMenu;
    [SerializeField]
    GameObject InventoryMenu;
    [SerializeField]
    GameObject InventoryPanel;
    [SerializeField]
    GameObject CraftPanel;

    public RectTransform inventorySelectionFrame;
	//public Button pauseButton;
	//public ButtonHandler jumpButton;
	//public Joystick mobileStick;

    //[SerializeField]
    //private GameObject quickInventoryPrefab;
    //private GameObject quickInventory;

    //private static RectTransform slotFrame;
    //private static int selectedSlotIndex = 0;

    //Menu Prefabs
 //   [SerializeField]
	//private GameObject pauseMenuPrefab;
 //   [SerializeField]
 //   private GameObject inventoryPrefab;

	//private variables
	private bool isPressedBack;

    //public static RectTransform SlotFrame
    //{
    //    get
    //    {
    //        return slotFrame;
    //    }

    //    set
    //    {
    //        slotFrame = value;
    //    }
    //}

    //public static int SelectedSlotIndex
    //{
    //    get
    //    {
    //        return selectedSlotIndex;
    //    }

    //    set
    //    {
    //        selectedSlotIndex = value;
    //    }
    //}

    //private GameObject activeMenu;

    // Use this for initialization
    void Start ()
	{
        uiController = GetComponent<UIController>();
        pauseMenu.SetActive(false);
        InventoryMenu.SetActive(false);
        quickInventoryMenu.SetActive(true);
		isPressedBack = false;
        //activeMenu = null;
        //slotFrame = GameObject.Find("GameUI/HUD/QuickInventory/SlotSelectedFrame").GetComponent<RectTransform>();
        //QuickInventoryMenuScript.SelectedSlotIndex = 0;
        //Instantiate(quickInventoryPrefab);
	}

	void Update ()
	{
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isPressedBack)
            {
                isPressedBack = true;
                if (uiController.activeMenu == pauseMenu)
                {
                    On_ContinueGameButton_Click();
                }
                else
                {
                    On_PauseButton_Click();
                }
            }
        }
        if (!Input.GetButtonDown("Cancel"))
        {
            isPressedBack = false;
        }
	}

	public void On_PauseButton_Click ()
	{
        pauseMenu.SetActive(true);
        uiController.activeMenu = pauseMenu;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
        //GameObject menu = Instantiate(pauseMenuPrefab);
        //menu.GetComponent<pauseMenuScript>().ParentScript = this;
	}

    public void On_ContinueGameButton_Click()
    {
        pauseMenu.SetActive(false);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = null;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }

    public void On_ExitButton_Click ()
	{
        //Message.showConfirmationMessage(2, ConfirmationMessageAction.QuitGame);
        exitGameConfirmationMenu.SetActive(true);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = exitGameConfirmationMenu;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
    }

    public void On_YesExitGameButton_Click()
    {
        SceneManager.LoadScene(0);
    }

    public void On_NoExitGameButtonClick()
    {
        exitGameConfirmationMenu.SetActive(false);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = pauseMenu;
    }

	public void On_SettingsButton_Click ()
	{
        //GameObject menu = Instantiate(settingsMenuPrefab);
        //menu.GetComponent<settingsMenuScript>().ParentScript = this;
	}

    public void On_OpenInventoryButton_Click()
    {
        quickInventoryMenu.SetActive(false);
        InventoryMenu.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
    }

    public void On_CloseInventoryButton_Click()
    {
        InventoryMenu.SetActive(false);
        quickInventoryMenu.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }

    public void On_OpenInventoryMenuButton_Click()
    {
        CraftPanel.SetActive(false);
        InventoryPanel.SetActive(true);
    }

    public void On_OpenCraftMenuButton_Click()
    {
        InventoryPanel.SetActive(false);
        CraftPanel.SetActive(true);
    }
}
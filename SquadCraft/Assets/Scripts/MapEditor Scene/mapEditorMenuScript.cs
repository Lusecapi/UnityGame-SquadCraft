using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public enum EditAction
{
    Draw,
    Erase,
    ClearAll
}

public class mapEditorMenuScript : MonoBehaviour
{
    //private GameObject editorPanel;
    [Header("Menu Panels")]
    [SerializeField]
    GameObject editorMenuPanel;
    [SerializeField]
    GameObject editorEditPanel;
    [SerializeField]
    GameObject exitEditorPanel;

    [Header("Require Elements from editor menu")]
    [SerializeField]
    private InputField worldSizeXInputField;
    [SerializeField]
    private InputField worldSizeYInputField;
    [SerializeField]
    private InputField worlNameInputField;
    [Header("Require Elements from Edit Panel")]
    [Range(0.01f, 1)]
    public float joystickSense = 0.2f;
    [SerializeField]
    private Slider zoomSlider;
    [SerializeField]
    private Text tileNameText;
    [SerializeField]
    private Button pencilToolButton;
    [SerializeField]
    private Button eraserToolButton;
    [SerializeField]
    private Button undoButton;
    [SerializeField]
    private Button clearAllButton;

    UIController uiController;

    private bool isPressedBack;
    public static bool isPencilSelected;
    public static bool isEraserSelected;
    public static bool wasUndo;
    public static bool wasClearAll;

    public static EditAction lastEditAction;
    public static List<TileInWorldSpace> lastDeletedTiles = new List<TileInWorldSpace>();
    public static List<TileInWorldSpace> tilesPlaced = new List<TileInWorldSpace>();

    public static RectTransform targetTile;
    public static string targetTileName;
    public static RectTransform frame;

    public static Vector2 maxCameraDistance;

    //   private static Color errorColor = new Color (1, 84 / 255.0F, 84 / 255.0F, 1);

    private int worldSizeX;
    private int worldSizeY;

    void Start ()
	{
        //editorPanel = Resources.Load("Menus/MapEditorEditPanel") as GameObject;
        uiController = GetComponent<UIController>();
        targetTile = GameObject.Find("MapEditorSceneUICanvas/EditorPanel/Panel/Scroll View/Viewport/Content/DirtGrass").GetComponent<RectTransform>();
        frame = GameObject.Find("MapEditorSceneUICanvas/EditorPanel/Panel/Scroll View/Viewport/Content/Frame").GetComponent<RectTransform>();
        editorEditPanel.SetActive(false);
        exitEditorPanel.SetActive(false);
        editorMenuPanel.SetActive(true);
        uiController.activeMenu = editorMenuPanel;
        isPressedBack = false;
        isPencilSelected = true;
        isEraserSelected = false;
        wasUndo = true;
        wasClearAll = true;
        targetTileName = "Dirt Grass";
        tileNameText.text = targetTileName;
    }

    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (!isPressedBack)
            {
                isPressedBack = true;
                if (uiController.activeMenu == editorMenuPanel)
                {
                    On_ExitButton_Click();
                }
                else
                    if (uiController.activeMenu == editorEditPanel)
                {
                    On_BackButton_Click();
                }
                else
                    if(uiController.activeMenu == exitEditorPanel)
                {
                    On_NoExitEditorButton_Click();
                }
            }
        }
        if (!Input.GetButtonDown("Cancel"))
        {
            isPressedBack = false;
        }
        

        if(uiController.activeMenu == editorEditPanel)
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            tileNameText.text = targetTileName;
            pencilToolButton.interactable = !isPencilSelected;
            eraserToolButton.interactable = !isEraserSelected;
            undoButton.interactable = !wasUndo;
            clearAllButton.interactable = !wasClearAll;

            Camera.main.transform.Translate(h * joystickSense, v * joystickSense, 0);
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, 0, maxCameraDistance.x), Mathf.Clamp(Camera.main.transform.position.y, 0, maxCameraDistance.y), Camera.main.transform.position.z);

        }
    }

    public void On_GenerateButton_Click ()
	{
		if (worldSizeXInputField.text != "" && worldSizeYInputField.text != "" ) {
			worldSizeX = Int32.Parse (worldSizeXInputField.text);
			worldSizeY = Int32.Parse (worldSizeYInputField.text);
            editorMenuPanel.SetActive(false);
            editorEditPanel.SetActive(true);
            uiController.beforeMenu = uiController.activeMenu;
            uiController.activeMenu = editorEditPanel;
            MapEditor.generateBackTiles(worldSizeX, worldSizeY);

        } else {

            Message.showMessageText("Verif World Sizes", MessageType.Error);
		}
	}

    public void On_ExitButton_Click()
    {
        SceneManager.LoadScene(0);
    }

    //Editor Edit Panel

    public void On_BackButton_Click()
    {
        //Message.showConfirmationMessage(1, ConfirmationMessageAction.QuitEditing);
        exitEditorPanel.SetActive(true);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = exitEditorPanel;
    }

    public void On_NoExitEditorButton_Click()
    {
        exitEditorPanel.SetActive(false);
        uiController.beforeMenu = uiController.activeMenu;
        uiController.activeMenu = editorEditPanel;
    }

    public void On_YesExitButtonEditorClick()
    {
        SceneManager.LoadScene(0);
    }

    public void On_SaveWorldButton_Click()
    {
        MapEditor.saveWorld(tilesPlaced, worldSizeX, worldSizeY, worlNameInputField.text);
    }


    public void On_PencilToolButton_Click()
    {
        if (!isPencilSelected)
        {
            isPencilSelected = true;
            isEraserSelected = false;
        }
    }

    public void On_EraserToolButton_Click()
    {
        if (!isEraserSelected)
        {
            isEraserSelected = true;
            isPencilSelected = false;
        }
    }

    public void On_ClearAllToolButton_Click()
    {
        wasClearAll = true;
        wasUndo = false;
        lastEditAction = EditAction.ClearAll;
        lastDeletedTiles.Clear();
        lastDeletedTiles.AddRange(tilesPlaced);//initialize with values and not with pointer
        for (int i = 0; i < tilesPlaced.Count; i++)
        {
            Destroy(tilesPlaced[i].TileGameObject);
        }
        tilesPlaced.Clear();
    }

    public void On_UndoToolButton_Click()
    {
        if (!wasUndo)
        {
            wasUndo = true;
            if (lastEditAction.Equals(EditAction.Draw))
            {
                Destroy(tilesPlaced[tilesPlaced.Count - 1].TileGameObject);
                tilesPlaced.RemoveAt(tilesPlaced.Count - 1);
            }
            else
                if (lastEditAction.Equals(EditAction.Erase))
            {
                wasClearAll = false;
                GameObject tile = Instantiate(Resources.Load("Tiles/" + lastDeletedTiles[0].TileName), lastDeletedTiles[0].Position, Quaternion.identity) as GameObject;
                tile.tag = "Tile in Editor";
                tile.AddComponent<BoxCollider2D>();
                tile.AddComponent<TileInEditor>();
                tilesPlaced.Add(new TileInWorldSpace(lastDeletedTiles[0].TileName, lastDeletedTiles[0].Position, tile));
            }
            else
                if (lastEditAction.Equals(EditAction.ClearAll))
            {
                wasClearAll = false;
                for (int i = 0; i < lastDeletedTiles.Count; i++)
                {
                    lastDeletedTiles[i].TileGameObject = Instantiate(Resources.Load("Tiles/" + lastDeletedTiles[i].TileName), lastDeletedTiles[i].Position, Quaternion.identity) as GameObject;
                    lastDeletedTiles[i].TileGameObject.tag = "Tile in Editor";
                    lastDeletedTiles[i].TileGameObject.AddComponent<BoxCollider2D>();
                    lastDeletedTiles[i].TileGameObject.AddComponent<TileInEditor>();


                }
                tilesPlaced.Clear();
                tilesPlaced.AddRange(lastDeletedTiles);
                lastDeletedTiles.Clear();

            }
        }
    }


    public void On_ZoomSlider_Move()
    {
        Camera.main.orthographicSize = zoomSlider.value;
    }

    private void cleanScene()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile in Editor");
        for (int i = 0; i < tiles.Length; i++)
        {
            Destroy(tiles[i]);
        }
    }




    public class TileInWorldSpace
    {
        private string tileName;
        private Vector3 position;
        private GameObject tileGameObject;

        #region Getters and Setters
        public string TileName
        {
            get { return tileName; }

            set { tileName = value; }
        }

        public Vector3 Position
        {
            get { return position; }

            set { position = value; }
        }

        public GameObject TileGameObject
        {
            get { return tileGameObject; }

            set { tileGameObject = value; }
        }

        #endregion

        public TileInWorldSpace(string theName, Vector3 thePosition, GameObject theGameObject)
        {
            TileName = theName;
            Position = thePosition;
            TileGameObject = theGameObject;
        }
    }
}

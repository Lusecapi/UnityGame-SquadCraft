using UnityEngine;
using System.Collections;

public class InventoryPanel : MonoBehaviour {
    
    public InventorySlot selectedSlot;
    public ItemInInventorySlot selectedItem;
    public InventorySlot originalSlot;
    public bool isDragging;

    private RectTransform selectionFrame;
    private int selectedSlotIndex;

    private GameObject itmeInSlotPrefab;

    public Character theCharacter;

    #region Getters and Setters

    public GameObject ItmeInSlotPrefab
    {
        get
        {
            return itmeInSlotPrefab;
        }
    }

    public RectTransform SelectionFrame
    {
        get
        {
            return selectionFrame;
        }
    }

    public int SelectedSlotIndex
    {
        get
        {
            return selectedSlotIndex;
        }

        set
        {
            selectedSlotIndex = value;
        }
    }

    #endregion

    // Use this for initialization
    void Start () {

        itmeInSlotPrefab = Resources.Load("Others/ItemInInventorySlot") as GameObject;
        selectionFrame = GetComponentInParent<gameUIScript>().inventorySelectionFrame;
        selectedSlotIndex = 0;
        theCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }
	
	// Update is called once per frame
	void Update () {

	}
}

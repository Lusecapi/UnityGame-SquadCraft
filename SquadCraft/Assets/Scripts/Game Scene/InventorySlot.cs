using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum SlotType
{
    QuickInventory,
    Inventory
}

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField]
    private int slotIndex;
    [SerializeField]
    private SlotType slotInWhat;

    private bool slotHasItemStored;
    private bool itemIsInHand;

    private InventoryPanel inventoryPanelParent;

    private RectTransform selectionFrame;

    public int SlotIndex
    {
        get
        {
            return slotIndex;
        }
    }

    public bool SlotHasItemStored
    {
        get
        {
            return slotHasItemStored;
        }

        set
        {
            slotHasItemStored = value;
        }
    }

    public SlotType SlotInWhat
    {
        get
        {
            return slotInWhat;
        }
    }

    //[SerializeField]
    //private Item objectStored;

    //public Image objectImage;


    // Use this for initialization
    void Start () {

        inventoryPanelParent = transform.parent.GetComponent<InventoryPanel>();
        //objectImage = transform.GetChild(0).GetComponent<Image>();
    }

    void OnEnable()
    {
        slotHasItemStored = false;
        itemIsInHand = false;
        if (transform.childCount == 1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryPanelParent.theCharacter)//If there is a player, fixing a bug
        {
            if (!slotHasItemStored && inventoryPanelParent.theCharacter.PlayerInventory[slotIndex] != null)
            {
                slotHasItemStored = true;
                GameObject iis = Instantiate(inventoryPanelParent.ItmeInSlotPrefab);
                iis.transform.SetParent(this.transform);
                iis.name = inventoryPanelParent.theCharacter.PlayerInventory[slotIndex].ItemName;
                iis.transform.localPosition = Vector3.zero;
                iis.GetComponent<RectTransform>().localScale = Vector3.one;
                iis.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                iis.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                iis.GetComponent<ItemInInventorySlot>().ItemName = inventoryPanelParent.theCharacter.PlayerInventory[slotIndex].ItemName;
                iis.GetComponent<ItemInInventorySlot>().setSprite(inventoryPanelParent.theCharacter.PlayerInventory[slotIndex].ItemSprite, inventoryPanelParent.theCharacter.PlayerInventory[slotIndex].ImageColor);
                iis.GetComponent<ItemInInventorySlot>().IsStackable = inventoryPanelParent.theCharacter.PlayerInventory[slotIndex].ItemIsStackable;


                if (inventoryPanelParent.SelectedSlotIndex == slotIndex)
                {
                    itemIsInHand = true;
                    inventoryPanelParent.theCharacter.setObjectInHand(slotIndex);
                }
            }
        }

        if (inventoryPanelParent.SelectedSlotIndex != slotIndex)
        {
            itemIsInHand = false;
        }

        if (!itemIsInHand && inventoryPanelParent.SelectedSlotIndex == slotIndex)
        {
            itemIsInHand = true;
            if (inventoryPanelParent.theCharacter)//if there is a player, fixing a bug
            {
                inventoryPanelParent.theCharacter.setObjectInHand(slotIndex);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotInWhat.Equals(SlotType.QuickInventory))
        {
            inventoryPanelParent.SelectionFrame.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            inventoryPanelParent.SelectedSlotIndex = this.slotIndex;
            inventoryPanelParent.theCharacter.selectedInventoryIndex = this.slotIndex;
            //    gameUIScript.SelectedSlotIndex = slotIndex;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       inventoryPanelParent.selectedSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryPanelParent.selectedSlot = null;
    }
}

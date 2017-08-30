using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class ItemInInventorySlot : MonoBehaviour, IPointerEnterHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler {

    private string itemName;
    private bool isStackable;
    private Text countText;

    private InventoryPanel inventoryPanelParent;
    private bool isPressed;
    private float pressedTime;

    public string ItemName
    {
        get
        {
            return itemName;
        }

        set
        {
            itemName = value;
        }
    }

    public bool IsStackable
    {
        get
        {
            return isStackable;
        }

        set
        {
            isStackable = value;
        }
    }

    public Text CountText
    {
        get
        {
            return countText;
        }

        set
        {
            countText = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        inventoryPanelParent = transform.parent.parent.GetComponent<InventoryPanel>();
        countText = GetComponentInChildren<Text>();
        if (!isStackable)
        {
            countText.gameObject.SetActive(false);
        }
        isPressed = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Buscar la forma de optimizar esto para que cambie siemmpre que se coja el item y no se verifique en el Update
        if (isStackable)
        {
            countText.text = inventoryPanelParent.theCharacter.PlayerInventory[transform.parent.GetComponent<InventorySlot>().SlotIndex].ItemQuantity.ToString();
            if(Int32.Parse(CountText.text) <= 0)
            {
                inventoryPanelParent.theCharacter.PlayerInventory[inventoryPanelParent.SelectedSlotIndex] = null;
                transform.parent.GetComponent<InventorySlot>().SlotHasItemStored = false;
                //transform.parent.GetComponent<InventorySlot>(). = false;
                inventoryPanelParent.theCharacter.setNextAvailableSlot();
                inventoryPanelParent.theCharacter.setObjectInHand(inventoryPanelParent.SelectedSlotIndex);
                Destroy(this.gameObject);
            }
        }

        if (isPressed && (Time.timeSinceLevelLoad - pressedTime )> 2)
        {
            //Drop the item
            inventoryPanelParent.theCharacter.dropItems(this, inventoryPanelParent.originalSlot.SlotIndex);
            inventoryPanelParent.originalSlot.SlotHasItemStored = false;
            if (inventoryPanelParent.originalSlot.SlotInWhat.Equals(SlotType.QuickInventory))
            {
                inventoryPanelParent.theCharacter.setObjectInHand(inventoryPanelParent.SelectedSlotIndex);
            }
            else
            {
                //do nothing
            }
            inventoryPanelParent.isDragging = false;
            Destroy(this.gameObject);
        }
    }

    public void setSprite(Sprite sprite, Color imageColor)
    {
        GetComponent<Image>().sprite = sprite;
        GetComponent<Image>().color = imageColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
        }

        if (!inventoryPanelParent.isDragging)
        {
            inventoryPanelParent.isDragging = true;
            this.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        }
        if (inventoryPanelParent.isDragging && inventoryPanelParent.selectedItem == this)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inventoryPanelParent.isDragging)
        {
            GetComponent<Collider2D>().enabled = false;
            inventoryPanelParent.originalSlot = transform.parent.GetComponent<InventorySlot>();
            transform.parent.SetSiblingIndex(0);
            isPressed = true;
            pressedTime = Time.timeSinceLevelLoad;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!inventoryPanelParent.isDragging)
        {
            inventoryPanelParent.selectedItem = this;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pressedTime = 0;
        
        //else
        //{
            if (inventoryPanelParent.isDragging && inventoryPanelParent.selectedItem == this)//were dragging this item and released
            {
                //if (inventoryPanelParent.selectedSlot.GetComponent<InventorySlot>().SlotInWhat.Equals(SlotType.Drop))
                //{
                    ////Drop the item
                    //inventoryPanelParent.theCharacter.dropItems(this, inventoryPanelParent.originalSlot.SlotIndex);
                    //inventoryPanelParent.originalSlot.SlotHasItemStored = false;
                    //if (inventoryPanelParent.originalSlot.SlotInWhat.Equals(SlotType.QuickInventory))
                    //{
                    //    inventoryPanelParent.theCharacter.setObjectInHand(inventoryPanelParent.SelectedSlotIndex);
                    //}
                    //else
                    //{
                    //    //do nothing
                    //}
                    //inventoryPanelParent.isDragging = false;
                    //Destroy(this.gameObject);
                //}
                //else
                //{
                    if (inventoryPanelParent.selectedSlot.SlotInWhat.Equals(SlotType.QuickInventory))
                    {
                        if (inventoryPanelParent.originalSlot.SlotIndex == inventoryPanelParent.SelectedSlotIndex)//the selected index moves with tha frame
                        {
                            inventoryPanelParent.SelectionFrame.anchoredPosition = inventoryPanelParent.selectedSlot.GetComponent<RectTransform>().anchoredPosition;
                            inventoryPanelParent.SelectedSlotIndex = inventoryPanelParent.selectedSlot.GetComponent<InventorySlot>().SlotIndex;
                        }
                        else
                            if (inventoryPanelParent.selectedSlot.SlotIndex == inventoryPanelParent.SelectedSlotIndex)
                        {
                            inventoryPanelParent.SelectionFrame.anchoredPosition = inventoryPanelParent.originalSlot.GetComponent<RectTransform>().anchoredPosition;
                            inventoryPanelParent.SelectedSlotIndex = inventoryPanelParent.originalSlot.GetComponent<InventorySlot>().SlotIndex;
                        }
                    }
                //}

                //if (!inventoryPanelParent.selectedSlot.SlotInWhat.Equals(SlotType.Drop))
                //{

                    bool swapItem = false;
                    if (inventoryPanelParent.selectedSlot.transform.childCount == 1)//has Child (item stored), must swap items slots
                    {
                        swapItem = true;
                        inventoryPanelParent.selectedSlot.transform.GetChild(0).SetParent(inventoryPanelParent.originalSlot.transform);
                    }
                    inventoryPanelParent.selectedItem.transform.SetParent(inventoryPanelParent.selectedSlot.transform);

                    if (swapItem)//if had to swap items
                    {
                        inventoryPanelParent.originalSlot.transform.GetChild(0).localPosition = Vector3.zero;
                    }
                    else
                    {
                        inventoryPanelParent.originalSlot.SlotHasItemStored = false;
                        inventoryPanelParent.selectedSlot.SlotHasItemStored = true;
                    }

                    //swap on inventory. always must to do
                    inventoryPanelParent.theCharacter.swapInventoryItems(inventoryPanelParent.selectedSlot.SlotIndex, inventoryPanelParent.originalSlot.SlotIndex);
                    inventoryPanelParent.theCharacter.setNextAvailableSlot();
                    //        }
                    inventoryPanelParent.selectedItem.transform.localPosition = Vector3.zero;
                    inventoryPanelParent.selectedItem.GetComponent<Collider2D>().enabled = true;
                    inventoryPanelParent.isDragging = false;
                    inventoryPanelParent.originalSlot = null;
                    this.GetComponent<RectTransform>().localScale = Vector3.one;
                //}
            }
            else
                if (!inventoryPanelParent.isDragging)//Just Clicked this item (no drag)
            {
                if (inventoryPanelParent.selectedSlot.SlotInWhat.Equals(SlotType.QuickInventory))//it's on the quick inventory menu
                {
                    inventoryPanelParent.SelectionFrame.anchoredPosition = inventoryPanelParent.selectedSlot.GetComponent<RectTransform>().anchoredPosition;
                    inventoryPanelParent.SelectedSlotIndex = inventoryPanelParent.selectedSlot.SlotIndex;
                    inventoryPanelParent.theCharacter.selectedInventoryIndex = inventoryPanelParent.selectedSlot.SlotIndex;
                }
            }
            else
            {
                inventoryPanelParent.isDragging = false;
            }
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!inventoryPanelParent.isDragging)
        {
            inventoryPanelParent.selectedItem = null;
        }
    }
}

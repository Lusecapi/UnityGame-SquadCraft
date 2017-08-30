using UnityEngine;
using System.Collections.Generic;
using System;

public class Character : MonoBehaviour {

    public int lifePoints = 100;
    public float speed = 10f;
    public float jumpForce = 1000f;
    public float destroyLimitRadius = 5;
    public float airControlTime = 0.5f;

    [SerializeField]
    private Transform dropPoint;
    [SerializeField]
    private LayerMask whatIsGround;
    GameObject playerNameText;

    private InventoryItem[] playerInventory = new InventoryItem[28];
    public int selectedInventoryIndex;
    private int nextAvailableInventorySlotIndex = 0;

    private Transform ObjectsArm;
    private Vector3 objectsDefautlLocalPosition = new Vector3(0, -0.62f, 0);
    private Vector3 objectsDefautlLocalRotation = new Vector3(0, 0, 25);

    bool isFacingLeft;
    bool isGrounded;
    bool isInAirControl;
    bool isJumping;
    float jumpTime;
    Transform groundCheck;
    float groundedCheckRadius = 0.05f;    

    Rigidbody2D myRigidBody2d;
    Animator myAnimatorController;

    public InventoryItem[] PlayerInventory
    {
        get
        {
            return playerInventory;
        }
    }

    public int NextAvailableInventorySlotIndex
    {
        get
        {
            return nextAvailableInventorySlotIndex;
        }

        set
        {
            nextAvailableInventorySlotIndex = value;
        }
    }

    // Use this for initialization
    void Start() {
        
        myRigidBody2d = GetComponent<Rigidbody2D>();
        myAnimatorController = GetComponent<Animator>();
        groundCheck = transform.FindChild("Ground Check");

        if (transform.localScale.x == 1)
        {
            isFacingLeft = true;
        }
        else
        {
            isFacingLeft = false;
        }
        ObjectsArm = transform.FindChild("RightArm").transform;
        playerNameText = new GameObject("PlayerNameText");
        playerNameText.AddComponent<GUIText>();
        playerNameText.GetComponent<GUIText>().text = SettingsManager.UserName;
        playerNameText.GetComponent<GUIText>().anchor = TextAnchor.UpperCenter;
        playerNameText.GetComponent<GUIText>().alignment = TextAlignment.Center;
        playerNameText.GetComponent<GUIText>().pixelOffset = new Vector2(0, 35);
        //playerNameText.transform.SetParent(transform);
        selectedInventoryIndex = 0;
    } 
	
	// Update is called once per frame
	void Update () {

        Debug.Log(isInAirControl);
    }

    void FixedUpdate()
    {
        //isGrounded = Physics2D.OverlapCircleAll(groundCheck.position, groundedCheckRadius, whatIsGround);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundedCheckRadius, whatIsGround);

        //isGrounded = false;
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedCheckRadius, whatIsGround);
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    if (colliders[i].gameObject != gameObject)
        //    {
        //        isGrounded = true;
        //    }

        //}
        //isInAirControl = !isGrounded;
        if (!isGrounded)
        {
            if (!isJumping)
            {
                jumpTime = Time.timeSinceLevelLoad;
                isJumping = true;
            }
            if(Time.timeSinceLevelLoad - jumpTime < airControlTime)
            {
                isInAirControl = true;
            }
            else
            {
                isInAirControl = false;
            }
            
        }
        else
        {
            isInAirControl = false;
            isJumping = false;
            jumpTime = 0;
        }

        myAnimatorController.SetBool("Grounded", isGrounded);

        myAnimatorController.SetFloat("Vertical Speed", myRigidBody2d.velocity.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundedCheckRadius);
        Gizmos.DrawWireSphere(transform.position, destroyLimitRadius);
    }

    public void Move(float horizontal)
    {
        if (isGrounded || isInAirControl)
        {
            myAnimatorController.SetFloat("Speed", Mathf.Abs(horizontal));
            myRigidBody2d.velocity = new Vector2(horizontal * speed, myRigidBody2d.velocity.y);

        }

        playerNameText.transform.position = Camera.main.WorldToViewportPoint(transform.position);

        if (isFacingLeft && horizontal > 0)
        {
            flipSight();
        }
        if (!isFacingLeft && horizontal < 0)
        {
            flipSight();
        }
    }

    void flipSight()//Girar el personaje
    {
        isFacingLeft = !isFacingLeft;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Jump(bool jump)
    {
        if (isGrounded && myRigidBody2d.velocity.y == 0)
        {
            isJumping = jump;
            jumpTime = Time.timeSinceLevelLoad;
            myRigidBody2d.AddForce(new Vector2(0f, jumpForce));
        }
    }

    public void Attack(bool attack)
    {
        myAnimatorController.SetTrigger("Attack");
    }

    public bool inventoryHasFreeSlots()
    {
        if(nextAvailableInventorySlotIndex == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
        //for (int i = 0; i < playerInventory.Length; i++)
        //{
        //    if(playerInventory[i] == null)
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }

    public void pickUpItem(Item item)
    {
        if (inventroyContainItem(item.ItemName))
        {
            if (item.IsStackable)
            {
                List<int> itemIndexes = getItemIndexes(item.ItemName);//there can be different stacks of same item, so we keep all the indexes
                bool done = false;
                int i = 0;
                while (i < itemIndexes.Count && !done)
                {
                    if (stackIsNotFull(itemIndexes[i]))//stack is not full
                    {
                        done = true;
                        playerInventory[itemIndexes[i]].ItemQuantity++;
                        setNextAvailableSlot();
                        Destroy(item.gameObject);
                    }
                    i++;
                    //else//is stackable but 
                    //{
                        
                    //}
                }

                if (!done)
                {
                    if (inventoryHasFreeSlots())
                    {
                        done = true;
                        InventoryItem it = new InventoryItem(item.ItemName, item.ItemSprite, item.ImageColor, item.IsStackable, item.TypeOfItem);
                        playerInventory[nextAvailableInventorySlotIndex] = it;
                        setNextAvailableSlot();
                        Destroy(item.gameObject);
                    }
                    else
                    {
                        //do nothing, full inventory
                    }
                }

            }
            else//has the item but is not stackable
            {
                if (inventoryHasFreeSlots())//if has free slot
                {
                    InventoryItem i = new InventoryItem(item.ItemName, item.ItemSprite, item.ImageColor, item.IsStackable, item.TypeOfItem);
                    playerInventory[nextAvailableInventorySlotIndex] = i;
                    setNextAvailableSlot();
                    Destroy(item.gameObject);
                }
                else
                {
                    //do nothing, because its full inventory
                }
            }
        }
        else//No has the item
        {
            if (inventoryHasFreeSlots())
            {
                InventoryItem i = new InventoryItem(item.ItemName, item.ItemSprite, item.ImageColor, item.IsStackable,item.TypeOfItem);
                playerInventory[nextAvailableInventorySlotIndex] = i;
                setNextAvailableSlot();
                Destroy(item.gameObject);
            }
        }
    }

    public void dropItems(ItemInInventorySlot theItem, int itemIndex)
    {
        GameObject itemPrefab = Resources.Load("Items/" + theItem.ItemName) as GameObject;
        if (theItem.IsStackable)
        {
            for (int i = 0; i < Int32.Parse(theItem.CountText.text); i++)
            {
                GameObject it = Instantiate(itemPrefab, dropPoint.position, Quaternion.identity) as GameObject;
                it.GetComponent<Rigidbody2D>().AddForce(new Vector2(200 * transform.localScale.x * -1, 100));
            }
        }
        else
        {
            GameObject it = Instantiate(itemPrefab, dropPoint.position, Quaternion.identity) as GameObject;
            it.GetComponent<Rigidbody2D>().AddForce(new Vector2(200 * transform.localScale.x * -1, 100));
        }
        playerInventory[itemIndex] = null;
        setNextAvailableSlot();
    }

    private bool stackIsNotFull(int index)
    {
        if (playerInventory[index].ItemQuantity < 3)//128 is the limit for each stackable item
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private List<int> getItemIndexes(string itemName)
    {
        List<int> indexes = new List<int>();
        int i = 0;
        while(i < playerInventory.Length)
        {
            if (playerInventory[i] != null)
            {
                if (playerInventory[i].ItemName.Equals(itemName))
                {
                    indexes.Add(i);
                }
            }
            i++;
        }
        return indexes;
    }

    private bool inventroyContainItem(string itemName)
    {
        for (int i = 0; i < playerInventory.Length; i++)
        {
            if (playerInventory[i] != null)
            {
                if (playerInventory[i].ItemName.Equals(itemName))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void setNextAvailableSlot()
    {
        //if (inventoryHasFreeSlots())
        //{
        int i = 0;
        bool foundAvailable = false;
        while (i < playerInventory.Length && !foundAvailable)
        {

            if (playerInventory[i] == null)
            {
                foundAvailable = true;
            }
            else
            {
                i++;
            }
        }

        if (foundAvailable)
        {
            nextAvailableInventorySlotIndex = i;
        }
        else
        {
            nextAvailableInventorySlotIndex = -1;
        }
        //while (i < playerInventory.Length && playerInventory[i] != null)
        //{
        //    i++;
        //}
        //nextAvailableInventorySlotIndex = i;
        //}
        //else
        //{
        //    nextAvailableInventorySlotIndex = -1;
        //}
    }

    

    //private bool verifyIndexAvailablity(int index)
    //{
    //    if(playerInventory[index] == null)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public void swapInventoryItems(int item1Index, int item2Index)
    {
        var aux = playerInventory[item1Index];
        playerInventory[item1Index] = playerInventory[item2Index];
        playerInventory[item2Index] = aux;
    }

    public void setObjectInHand(int itemInventroyIndex)
    {
        if(playerInventory[itemInventroyIndex] != null)//the item something, no null
        {
            if (ObjectsArm.childCount == 1)//has child (one object)
            {
                Destroy(ObjectsArm.GetChild(0).gameObject);//We destroy it
            }
            GameObject theObjectPrefab = Resources.Load("Objects/" + playerInventory[itemInventroyIndex].ItemName) as GameObject;
            GameObject objectGameObject = Instantiate(theObjectPrefab);
            objectGameObject.transform.SetParent(ObjectsArm);
            objectGameObject.transform.localPosition = objectsDefautlLocalPosition;
            objectGameObject.transform.localRotation = Quaternion.Euler(objectsDefautlLocalRotation);
            objectGameObject.transform.localScale = setObjectScale(objectGameObject.transform, transform.localScale);
        }
        else
        {
            if(ObjectsArm.childCount == 1)//has object in hand
            {
                Destroy(ObjectsArm.GetChild(0).gameObject);//We destroy it
            }
            else
            {
                //no object in hand, do nothing
            }
        }
    }

    /// <summary>
    /// Adjust the object scale depending the side that player is looking
    /// </summary>
    /// <param name="theObjectTransform">The Object Transform</param>
    /// <returns>The correct scale of the object</returns>
    private Vector3 setObjectScale(Transform theObjectTransform, Vector3 playerScale)
    {
        var scale = new Vector3(-1,1,1);
        //if(playerScale.x < 0)
        //{
        //    scale.x = -1;
        //}
        //else if(playerScale.x > 0)
        //{
        //    scale.x =  -1;
        //}
        //scale.x = theObjectTransform.localScale.x * Mathf.Sign(playerScale.x);//This is a default configuration, and needs all objects look left side by default to work perfectly
        return scale;
    }



    public class InventoryItem
    {
        private string itemName;
        private Sprite itemSprite;
        private Color imageColor;
        private bool itemIsStackable;
        private ItemType typeOfItem;
        private int itemQuantity;

        public InventoryItem(string name, Sprite sprite, Color color, bool isStackable, ItemType itemType)
        {
            itemName = name;
            itemSprite = sprite;
            imageColor = color;
            itemIsStackable = isStackable;
            typeOfItem = itemType;
            itemQuantity = 1;

        }

        public string ItemName
        {
            get
            {
                return itemName;
            }
        }

        public Sprite ItemSprite
        {
            get
            {
                return itemSprite;
            }
        }

        public bool ItemIsStackable
        {
            get
            {
                return itemIsStackable;
            }
        }

        public int ItemQuantity
        {
            get
            {
                return itemQuantity;
            }

            set
            {
                itemQuantity = value;
            }
        }

        public Color ImageColor
        {
            get
            {
                return imageColor;
            }
        }

        public ItemType TypeOfItem
        {
            get
            {
                return typeOfItem;
            }
        }
    }
}

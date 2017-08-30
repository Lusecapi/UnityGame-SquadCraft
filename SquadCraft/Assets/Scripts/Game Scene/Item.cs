using UnityEngine;
using System.Collections;

public enum ItemType
{
    Food,
    Special,
    Placeable,
    Throwable,
    Tool,
    Weapon
}

public class Item : MonoBehaviour {

    [SerializeField]
    private string itemName;
    [SerializeField]
    private bool isStackable;
    [SerializeField]
    private ItemType typeOfItem;
    private Sprite itemSprite;
    private Color imageColor;

    public Sprite ItemSprite
    {
        get
        {
            return itemSprite;
        }
    }

    public string ItemName
    {
        get
        {
            return itemName;
        }
    }

    public bool IsStackable
    {
        get
        {
            return isStackable;
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

    // Use this for initialization
    void Start () {

        itemSprite = GetComponent<SpriteRenderer>().sprite;
        imageColor = GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Character Body"))
        {
            Character player = other.transform.parent.gameObject.GetComponent<Character>();
            player.pickUpItem(this);
        }
    }
}

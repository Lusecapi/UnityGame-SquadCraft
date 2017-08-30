using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    Character myCharacter;
    float horizontal;
    bool jump;
    bool attack;
    RaycastHit2D hit;

    int fingerId;
    bool clicked;
    float clickTime;
    Vector3 mousePosition;

	// Use this for initialization
	void Start () {

        myCharacter = GetComponent<Character>();
        jump = false;
        attack = false;
	}
	
	// Update is called once per frame
	void Update () {

        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        jump = CrossPlatformInputManager.GetButton("Jump");
        attack = CrossPlatformInputManager.GetButtonDown("Fire1");

#if UNITY_ANDROID && !UNITY_EDITOR


        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                clickTime = Time.timeSinceLevelLoad;
                clicked = true;
                mousePosition = touch.position;
                hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y), Vector2.zero, 0);
            }

            if(clicked && Time.timeSinceLevelLoad - clickTime > 0.3)
            {
                //LONG PRESS
                if (hit)
                {
                    if (hit.transform.tag.Equals("Tile"))
                    {
                        if (Vector2.Distance(myCharacter.transform.position, hit.transform.position) <= myCharacter.destroyLimitRadius)
                        {
                            Debug.Log("Esta en el Limite");
                            Tile tile = hit.transform.GetComponent<Tile>();
                            tile.PressedTile();
                        }
                        else
                        {
                            Debug.Log("Esta Muy Lejos");
                        }
                    }
                }
                clicked = false;//Because we dont want to execute this code anymore, just once
            }

            if(touch.phase == TouchPhase.Ended)
            {
                clicked = false;

                if(Time.timeSinceLevelLoad - clickTime < 0.3)
                {
                    //SHORT CLICK

                    //BUILD SYSTEM

                    if (hit)//if raycast hit something
                    {
                        //FALTA VERIFICAR QUE NO ESTE SOBRE UN UI
                        Vector2 mousePositionInWorldSpace = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);

                        if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Background Tile")))
                        {
                            //Instanciar el bloque, si no es otra background
                            if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                            {
                                if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                                {
                                    if (TileCodification.getTileLayer(myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) != LayerMask.NameToLayer("Background Tile"))
                                    {
                                        GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                        Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                        GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                        tile.GetComponent<Tile>().canPlaceOverIt = false;
                                        myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                        tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                        tile.transform.SetParent(GameObject.Find("World").transform);

                                        if (tile.layer.Equals(LayerMask.NameToLayer("Front Tile")))
                                        {
                                            tile.layer = LayerMask.NameToLayer("Ground");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("No puede construir tan lejos");
                            }
                        }
                        else
                            if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Special Tile")))
                        {
                            //Instanciar el bloque solo si es es una FrtonTile
                            if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                            {
                                if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                                {
                                    if (TileCodification.getTileLayer(myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) == LayerMask.NameToLayer("Front Tile"))//only if is a front tile
                                    {
                                        if (hit.transform.GetComponent<Tile>().canPlaceOverIt)
                                        {
                                            Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                            GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                            GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                            tile.GetComponent<Tile>().canPlaceOverIt = false;
                                            myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                            tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                            tile.transform.SetParent(GameObject.Find("World").transform);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("No puede construir tan lejos");
                            }

                        }
                        else
                        {
                            //Do nothing, can't instantiate tile over the other layers
                        }
                    }
                    else//not hit something
                    {
                        if (!EventSystem.current.IsPointerOverGameObject())//isn't over an UI element
                        {
                            //We transform de mouse position on a World position
                            Vector2 mousePositionInWorldSpace = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);

                            //We verify if the click is on player destry Limit radius
                            if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                            {
                                if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                                {
                                    GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                    Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                    GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                    myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                    tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                    tile.transform.SetParent(GameObject.Find("World").transform);
                                }
                            }
                            else
                            {
                                Debug.Log("No puede construir tan lejos");
                            }
                        }
                    }
                }
                else
                {
                    //WE RELEASE LONG PRESS
                    Debug.Log("End long click");
                    if (hit)
                    {
                        if (hit.transform.tag.Equals("Tile"))
                        {
                            if (Vector2.Distance(myCharacter.transform.position, hit.transform.position) <= myCharacter.destroyLimitRadius)
                            {
                                Tile tile = hit.transform.GetComponent<Tile>();
                                tile.ReleasedTile();
                            }
                        }

                    }
                }
            }
        }

#elif UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            clickTime = Time.timeSinceLevelLoad;
            clicked = true;
            mousePosition = Input.mousePosition;
            hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y), Vector2.zero, 0);
        }

        if (clicked && (Time.timeSinceLevelLoad - clickTime) > 0.3)
        {
            // long click effect
            Debug.Log("Long pressing");

            if (hit)
            {
                if (hit.transform.tag.Equals("Tile"))
                {
                    if (Vector2.Distance(myCharacter.transform.position, hit.transform.position) <= myCharacter.destroyLimitRadius)
                    {
                        Debug.Log("Esta en el Limite");
                        Tile tile = hit.transform.GetComponent<Tile>();
                        tile.PressedTile();
                    }
                    else
                    {
                        Debug.Log("Esta Muy Lejos");
                    }
                }
            }
            clicked = false;//Because we dont want to execute this code anymore, just once
        }

        if (Input.GetMouseButtonUp(0))
        {
            clicked = false;

            if ((Time.timeSinceLevelLoad - clickTime) < 0.3)
            {
                // short click effect
                Debug.Log("Short click");

                //BUILD SYSTEM

                if (hit)//if raycast hit something
                {
                    //FALTA VERIFICAR QUE NO ESTE SOBRE UN UI
                    Vector2 mousePositionInWorldSpace = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);

                    if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Background Tile")))
                    {
                        //Instanciar el bloque, si no es otra background
                        if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                        {
                            if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                            {
                                if (TileCodification.getTileLayer(myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) != LayerMask.NameToLayer("Backgroun Tile"))
                                {
                                    Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                    GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                    GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                    tile.GetComponent<Tile>().canPlaceOverIt = false;
                                    myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                    tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                    tile.transform.SetParent(GameObject.Find("World").transform);

                                    if (tile.layer.Equals(LayerMask.NameToLayer("Front Tile")))
                                    {
                                        tile.layer = LayerMask.NameToLayer("Ground");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("No puede construir tan lejos");
                        }
                    }
                    else
                        if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Special Tile")))
                    {
                        //Instanciar el bloque solo si es es una FrtonTile
                        if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                        {
                            if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                            {
                                if (TileCodification.getTileLayer(myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) == LayerMask.NameToLayer("Front Tile"))//only if is a front tile
                                {
                                    if (hit.transform.GetComponent<Tile>().canPlaceOverIt)
                                    {
                                        Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                        GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                        GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                        tile.GetComponent<Tile>().canPlaceOverIt = false;
                                        myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                        tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                        tile.transform.SetParent(GameObject.Find("World").transform);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("No puede construir tan lejos");
                        }

                    }
                    else
                    {
                        //Do nothing, can't instantiate tile over the other layers
                    }
                }
                else//not hit something
                {
                    if (!EventSystem.current.IsPointerOverGameObject())//isn't over an UI element
                    {
                        //We transform de mouse position on a World position
                        Vector2 mousePositionInWorldSpace = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);

                        //We verify if the click is on player destry Limit radius
                        if (Vector2.Distance(myCharacter.transform.position, mousePositionInWorldSpace) <= myCharacter.destroyLimitRadius)
                        {
                            if (myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex] != null && myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].TypeOfItem.Equals(ItemType.Placeable))
                            {
                                GameObject tilePrefab = Resources.Load("Tiles/" + myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName) as GameObject;
                                Vector3 objecPos = builtTilePosition(mousePositionInWorldSpace);
                                GameObject tile = Instantiate(tilePrefab, objecPos, Quaternion.identity) as GameObject;
                                myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemQuantity--;
                                tile.name = myCharacter.PlayerInventory[myCharacter.selectedInventoryIndex].ItemName;
                                tile.transform.SetParent(GameObject.Find("World").transform);
                            }
                        }
                        else
                        {
                            Debug.Log("No puede construir tan lejos");
                        }
                    }
                }
            }
            else
            {
                //We release Long press
                if (hit)
                {
                    if (hit.transform.tag.Equals("Tile"))
                    {
                        if (Vector2.Distance(myCharacter.transform.position, hit.transform.position) <= myCharacter.destroyLimitRadius)
                        {
                            Tile tile = hit.transform.GetComponent<Tile>();
                            tile.ReleasedTile();
                        }
                    }

                }
            }
        }

#endif

    }

    void FixedUpdate()
    {

        myCharacter.Move(horizontal);
        if (jump)
        {
            myCharacter.Jump(jump);
        }
        if (attack)
        {
            myCharacter.Attack(attack);
        }
    }

    private Vector3 builtTilePosition(Vector3 mouseWorldPoint)
    {
        float blockSize = 1.28f;
        int posX = tilePosNumberX(mouseWorldPoint.x);
        int posY = tilePosNumberX(mouseWorldPoint.y);
        return new Vector3(posX * blockSize, posY * blockSize, 0);
    }

    private int tilePosNumberX(float axisXPosition)
    {
        int f = (int)((axisXPosition + 0.63f) / 1.28f);
        return f;
    }

    private int tilePosNumberY(float axisYPosition)
    {
        int f = (int)((axisYPosition - 0.63) / 1.28f);
        return f;
    }
}

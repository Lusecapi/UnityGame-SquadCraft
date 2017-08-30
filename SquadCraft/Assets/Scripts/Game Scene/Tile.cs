using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    private int destroyTime = 1;
    [HideInInspector]
    public bool canPlaceOverIt = true;
    private int pressedTime;
    private int time;

    private bool isPressed;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Down");
        //isPressed = true;
        //pressedTime = (int)Time.timeSinceLevelLoad;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Up");
        //isPressed = false;
        //pressedTime = 0;
    }

    // Use this for initialization
    void Start() {

        time = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (isPressed)
        {
            time = (int)Time.timeSinceLevelLoad - pressedTime;
        }

        if (time >= destroyTime)
        {
            GameObject item = Resources.Load("Items/" + this.gameObject.name) as GameObject;
            Instantiate(item, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }

    public void PressedTile()
    {
        Debug.Log("Down");
        isPressed = true;
        pressedTime = (int)Time.timeSinceLevelLoad;
    }

    public void ReleasedTile()
    {
        Debug.Log("Up");
        isPressed = false;
        pressedTime = 0;
    }
}

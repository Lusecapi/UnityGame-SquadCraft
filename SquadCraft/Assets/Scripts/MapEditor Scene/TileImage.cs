using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class TileImage : MonoBehaviour, IPointerClickHandler {

    [SerializeField]
    private string tileName;
    private static bool isEnable = true;

    public static bool IsEnable
    {
        get
        {
            return isEnable;
        }

        set
        {
            isEnable = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEnable)
        {
            mapEditorMenuScript.targetTile = GetComponent<RectTransform>();
            mapEditorMenuScript.targetTileName = tileName;
            mapEditorMenuScript.frame.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}

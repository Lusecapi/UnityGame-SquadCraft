using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class playWorldButtonScript : MonoBehaviour, IPointerClickHandler {

    worldButtonScript parentButon;

    // Use this for initialization
    void Start()
    {
        parentButon = GetComponentInParent<worldButtonScript>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WorldGenerator.FileName = parentButon.WorldFile;
        WorldGenerator.TheWorldType = parentButon.WorldType;
        SceneManager.LoadScene(1);
    }
}

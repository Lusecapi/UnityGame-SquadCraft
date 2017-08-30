using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;

public class editWorldButtonScript : MonoBehaviour, IPointerClickHandler
{
    worldButtonScript parentButton;

    void Start()
    {
        parentButton = GetComponentInParent<worldButtonScript>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //parentButton.ParentScript.SwitchEnable();
        GameObject g = Instantiate(parentButton.editWorldMenuPrefab);
        g.GetComponent<editWorldMenuScript>().WorldFile = parentButton.WorldFile;
        //g.GetComponent<editWorldMenuScript>().ParentScript = parentButton.ParentScript;
    }
}

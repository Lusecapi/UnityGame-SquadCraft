using UnityEngine;
using System.Collections;

public class CraftPanel : MonoBehaviour {

    Character thePlayer;

	// Use this for initialization
	void Start () {

        thePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

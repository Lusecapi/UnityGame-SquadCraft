using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private GameObject followingTarget;
    [SerializeField]
    private float followingTime = 1f;
    [SerializeField]
    //private int variable = 70;

    private Camera myCamera;

    private bool hasTaget;

    public GameObject FollowingTarget
    {
        get
        {
            return followingTarget;
        }

        set
        {
            followingTarget = value;
        }
    }

    // Use this for initialization
    void Start()
    {

        myCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        scaling();
        transform.position = Vector3.Lerp(transform.position, followingTarget.transform.position, followingTime) + new Vector3(0f, 0f, -10f);
    }

    private void scaling()
    {
        //myCamera.orthographicSize = Screen.height / variable;
        myCamera.orthographicSize = 5;
    }
}

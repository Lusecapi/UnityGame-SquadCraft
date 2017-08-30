using UnityEngine;
using UnityEngine.UI;

public class worldButtonScript : MonoBehaviour
{

    [SerializeField]
    private string worldFile;
    //private RootMenu parentScript;
    private WorldGenerator.WorldType worldType = WorldGenerator.WorldType.GameWorld;//This value is by default, beacuse it first is as world button of a game world, but if it is one of my world, the value is modifyed on WorldButton Class inside of plamenuScript
    private playWorldButtonScript playButton;
    private editWorldButtonScript editButton;
    public GameObject editWorldMenuPrefab;

    #region Getter and Setter
    public string WorldFile
    {
        get
        {
            return worldFile;
        }

        set
        {
            worldFile = value;
        }
    }

    //public RootMenu ParentScript
    //{
    //    get
    //    {
    //        return parentScript;
    //    }

    //    set
    //    {
    //        parentScript = value;
    //    }
    //}

    public WorldGenerator.WorldType WorldType
    {
        get
        {
            return worldType;
        }

        set
        {
            worldType = value;
        }
    }

    public editWorldButtonScript EditButton
    {
        get
        {
            return editButton;
        }

        set
        {
            editButton = value;
        }
    }

    public playWorldButtonScript PlayButton
    {
        get
        {
            return playButton;
        }

        set
        {
            playButton = value;
        }
    }
    #endregion

    void Start()
    {
        playButton = GetComponentInChildren<playWorldButtonScript>();
        editButton = GetComponentInChildren<editWorldButtonScript>();
        editWorldMenuPrefab = Resources.Load("Menus/EditWorldMenu") as GameObject;
    }
}
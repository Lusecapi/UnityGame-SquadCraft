using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class WorldGenerator : MonoBehaviour
{

    private static string fileName= "Level1";
    private static WorldType theWorldType;
    private static float blocksSize = 1.28f;

    private List<Transform> spawnPointsList;

    public enum WorldType
    {
        GameWorld,
        MyWorld
    }


    #region Getters And Setters

    public static string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }

    public static WorldType TheWorldType
    {
        get
        {
            return theWorldType;
        }

        set
        {
            theWorldType = value;
        }
    }



    #endregion

    void Awake()
    {
        spawnPointsList = new List<Transform>();
        readWorldFile(fileName, theWorldType);
        spawnPlayer();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Load the world
    /// </summary>
    /// <param name="fileName">The world file </param>
    /// <param name="worldType">the world type (if GameWorld or MyWorld)</param>
    void readWorldFile(string fileName, WorldType worldType)
    {
        GameObject worldGameObject = new GameObject("World");
        if (worldType.Equals(WorldType.MyWorld))
        {
            StreamReader sr = new StreamReader(MapEditor.WorldsFilesPath + "/" + fileName);
            string line = sr.ReadLine();
            string[] worldParameters = line.Split(';');
            string[] backgrounds = sr.ReadLine().Split(';');
            int rows = int.Parse(worldParameters[0].Split('x')[0]);//if needs
            int columns = int.Parse(worldParameters[0].Split('x')[1]);
            setBackground(backgrounds[0], backgrounds[1], columns, rows);
            int row = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                string[] vec = line.Split(';');
                for (int colum = 0; colum < columns; colum++)
                {
                    string tile1Code = vec[colum].Split('-')[0];
                    string tile2Code = vec[colum].Split('-')[1];
                    if (!tile1Code.Equals("0"))
                    {

                        //No hay forma que el tileCode sea un spawn point, eso se controla en la creacion del mundo y en el archivo, entonces instacinamos con seguridad
                        string tileName = TileCodification.getTileName(tile1Code);
                        GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                        GameObject tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows-1) - row), 0f), Quaternion.identity) as GameObject;
                        tile.name = tileName;
                        tile.transform.SetParent(worldGameObject.transform);

                        if (!tile2Code.Equals("0"))
                        {
                            //Aqui si puede haber spawn Points
                            if(!tile2Code.Equals(TileCodification.getTileCode("Spawn Point")))
                            {
                                tileName = TileCodification.getTileName(tile2Code);
                                tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                                tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f), Quaternion.identity) as GameObject;
                                tile.name = tileName;
                                tile.transform.SetParent(worldGameObject.transform);

                                if (tile1Code.Substring(0, 1) == "b" && tile2Code.Substring(0,1) == "f")//also could be use TileCodification.getTileLayer(TileCodification.getTileName(tile1Code)) to get the Tiles layers
                                {
                                    tile.layer = LayerMask.NameToLayer("Ground");
                                }
                                
                            }
                            else
                            {
                                GameObject sp = new GameObject("Spawn Point " + spawnPointsList.Count);
                                sp.transform.position = new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f);
                                spawnPointsList.Add(sp.transform);
                            }
                        }
                        else
                        {
                            //Instantiate nothing
                        }
                    }
                    else
                    {
                        if (!tile2Code.Equals("0"))
                        {
                            if (!tile2Code.Equals(TileCodification.getTileCode("Spawn Point")))
                            {
                                string tileName = TileCodification.getTileName(tile2Code);
                                GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                                GameObject tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f), Quaternion.identity) as GameObject;
                                tile.name = tileName;
                                tile.transform.SetParent(worldGameObject.transform);
                            }
                            else
                            {
                                GameObject sp = new GameObject("Spawn Point " + spawnPointsList.Count);
                                sp.transform.position = new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f);
                                spawnPointsList.Add(sp.transform);
                            }
                        }
                        else
                        {
                            //Instantiate nothing
                        }
                    }
                }
                row++;
            }
            sr.Close();
        }
        else
        {
            //Reading a txt file inside resources folder of the project in adroid build (Android), also works on editor so no problem
            TextAsset path = Resources.Load<TextAsset>("Files/World Files/" + fileName);
            using (StreamReader sr = new StreamReader(new MemoryStream(path.bytes)))
            {
                string line = sr.ReadLine();
                string[] worldParameters = line.Split(';');
                string[] backgrounds = sr.ReadLine().Split(';');
                int rows = int.Parse(worldParameters[0].Split('x')[0]);//if needs
                int columns = int.Parse(worldParameters[0].Split('x')[1]);
                setBackground(backgrounds[0], backgrounds[1], columns, rows);
                int row = 0;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] vec = line.Split(';');
                    for (int colum = 0; colum < columns; colum++)
                    {
                        string tile1Code = vec[colum].Split('-')[0];
                        string tile2Code = vec[colum].Split('-')[1];
                        if (!tile1Code.Equals("0"))
                        {

                            //No hay forma que el tileCode sea un spawn point, eso se controla en la creacion del mundo y en el archivo, entonces instacinamos con seguridad
                            string tileName = TileCodification.getTileName(tile1Code);
                            GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                            GameObject tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f), Quaternion.identity) as GameObject;
                            tile.name = tileName;
                            tile.transform.SetParent(worldGameObject.transform);

                            if (!tile2Code.Equals("0"))
                            {
                                //Aqui si puede haber spawn Points
                                if (!tile2Code.Equals(TileCodification.getTileCode("Spawn Point")))
                                {
                                    tileName = TileCodification.getTileName(tile2Code);
                                    tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                                    tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f), Quaternion.identity) as GameObject;
                                    tile.name = tileName;
                                    tile.transform.SetParent(worldGameObject.transform);

                                    if (tile1Code.Substring(0, 1) == "b" && tile2Code.Substring(0, 1) == "f")//also could be use TileCodification.getTileLayer(TileCodification.getTileName(tile1Code)) to get the Tiles layers
                                    {
                                        tile.layer = LayerMask.NameToLayer("Ground");
                                    }

                                }
                                else
                                {
                                    GameObject sp = new GameObject("Spawn Point " + spawnPointsList.Count);
                                    sp.transform.position = new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f);
                                    spawnPointsList.Add(sp.transform);
                                }
                            }
                            else
                            {
                                //Instantiate nothing
                            }
                        }
                        else
                        {
                            if (!tile2Code.Equals("0"))
                            {
                                if (!tile2Code.Equals(TileCodification.getTileCode("Spawn Point")))
                                {
                                    string tileName = TileCodification.getTileName(tile2Code);
                                    GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
                                    GameObject tile = Instantiate(tilePrefab, new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f), Quaternion.identity) as GameObject;
                                    tile.name = tileName;
                                    tile.transform.SetParent(worldGameObject.transform);
                                }
                                else
                                {
                                    GameObject sp = new GameObject("Spawn Point " + spawnPointsList.Count);
                                    sp.transform.position = new Vector3(blocksSize * colum, blocksSize * ((rows - 1) - row), 0f);
                                    spawnPointsList.Add(sp.transform);
                                }
                            }
                            else
                            {
                                //Instantiate nothing
                            }
                        }
                    }
                    row++;
                }
                sr.Close();
            }
        }
    }


    /// <summary>
    /// Spawn player character at a random world spawn point
    /// </summary>
    void spawnPlayer()
    {
        GameObject player = Resources.Load("Characters/" + SettingsManager.UserGender.ToString() + "Character") as GameObject;
        int spawnPointPostionIndex = Random.Range(0, spawnPointsList.Count);//For some reason The range return a value from 0 to spawnpoint list coutn -1, seems like it's an error on the documentation and maxValue isn't inclusive as it says.
        GameObject playerInstance = (GameObject)Instantiate(player, spawnPointsList[spawnPointPostionIndex].position, Quaternion.identity);
        Camera.main.GetComponent<CameraFollow>().FollowingTarget = playerInstance;
        //ObjectLabel.target = playerInstance.transform;
    }

    void setBackground(string topBackground, string bottomBackgroun, int x,int y)
    {
        GameObject top = Resources.Load("Backgrounds/" + topBackground) as GameObject;
        GameObject bottom = Resources.Load("Backgrounds/" + bottomBackgroun) as GameObject;
        //EL top Background ocupa el 50% del mundo y el bottom el 50%

        GameObject tb = Instantiate(top);
        tb.transform.position = new Vector2(0, blocksSize * y-0.1f);
        tb.transform.localScale = new Vector3(1/(5.12f / blocksSize) * x, 1/(5.12f / blocksSize) *(y/2), 1);//el 5.12 es lo que mide la imagen de los fondos y con esa division hacemos que mida 1.28

        GameObject bb = Instantiate(bottom);
        bb.transform.position = new Vector2(0, blocksSize * (y / 2));
        bb.transform.localScale = new Vector3(1/(5.12f / blocksSize) * x, 1/(5.12f / blocksSize) * (y / 2), 1);


    }
}
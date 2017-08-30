using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public static class MapEditor
{
    //Private
    private static float blockSize = 1.28f;
    private static string worldsFilesPath;

    public static string WorldsFilesPath
    {
        get{ return worldsFilesPath; }
        set{ worldsFilesPath = value; }
    }

    public static void setWorldsFilesPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        worldsFilesPath = "/storage/emulated/0/SquadCraft/My Worlds";
#elif UNITY_EDITOR
        worldsFilesPath = Application.dataPath + "/Resources/Files/My Worlds";
#endif
    }

    public static void generateBackTiles(int sizeX, int sizeY)
    {
        GameObject backTiles = new GameObject("Back Tiles ");
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                GameObject backTilePrefab = Resources.Load("Tiles/Back Tile") as GameObject;
                GameObject backTile = MonoBehaviour.Instantiate(backTilePrefab, new Vector2(j * blockSize, i * blockSize), Quaternion.identity) as GameObject;
                backTile.transform.SetParent(backTiles.transform);
            }
        }
        mapEditorMenuScript.maxCameraDistance = new Vector2(sizeX * blockSize, sizeY * blockSize);
    }

    /// <summary>
    /// Saves the world file at worlds folder
    /// </summary>
    /// <param name="tilesPlaced">The List of tiles placed at the world</param>
    /// <param name="sizeX">Number of Columns</param>
    /// <param name="sizeY">Number of rowns</param>
    /// <param name="worldName">The world Name (World file name)</param>
    public static void saveWorld(List<mapEditorMenuScript.TileInWorldSpace> tilesPlaced, int sizeX, int sizeY, string worldName)
    {
        if (Directory.Exists(worldsFilesPath))//If the folder already exist
        {
            if (worldName != "")
            {


#if UNITY_ANDROID && !UNITY_EDITOR
            string worldPath = worldsFilesPath + "/" + worldName + ".txt";
#elif UNITY_EDITOR
                string worldPath = worldsFilesPath + "\\" + worldName + ".txt";
#endif
                if (!File.Exists(worldPath))//If there isn't the same world
                {
                    //Debug.Log("Bien, el mundo no existe");
                    if (verifHasSpawnPoints(tilesPlaced))//If there is at least one spawn point
                    {
                        //Debug.Log("Tiene Spawn Point");
                        try
                        {
                            StreamWriter sw = new StreamWriter(worldPath);
                            sw.WriteLine(sizeY + "x" + sizeX);
                            sw.WriteLine("Sky;Underground");//Top background and bottom background

                            for (int i = sizeY - 1; i >= 0; i--)
                            {
                                string line = string.Empty;
                                for (int j = 0; j <= sizeX - 1; j++)
                                {
                                    bool swt1 = false;
                                    int k = 0;
                                    while (k < tilesPlaced.Count && swt1 == false)
                                    {
                                        if (tilesPlaced[k].Position.Equals(new Vector3(blockSize * j, blockSize * i, 0)))//Look for the actual tile, placed at the same position
                                        {
                                            swt1 = true;
                                        }
                                        else
                                        {
                                            k++;
                                        }
                                    }
                                    if (swt1)//If found one
                                    {
                                        if (TileCodification.getTileLayer(tilesPlaced[k].TileName) != LayerMask.NameToLayer("Front Tile"))//Solo se revisa si hay otro tile en la misma psicion si no es un Front Tile
                                        {


                                            //We make the second Search omiting the one we found
                                            bool swt2 = false;
                                            int l = 0;
                                            while (l < tilesPlaced.Count && !swt2)
                                            {
                                                if (tilesPlaced[l].Position.Equals(new Vector3(blockSize * j, blockSize * i, 0)) && l != k)//Look for the actual tile, placed at the same position
                                                {
                                                    swt2 = true;
                                                }
                                                else
                                                {
                                                    l++;
                                                }
                                            }

                                            if (swt2)//Encontro otra tile en la misma posicion
                                            {

                                                string tile1Code = TileCodification.getTileCode(tilesPlaced[k].TileName);
                                                string tile2Code = TileCodification.getTileCode(tilesPlaced[l].TileName);
                                                if (tile1Code.Substring(0, 1) == "b" && (tile2Code.Substring(0, 1) == "s" || tile2Code.Substring(0, 1) == "f"))
                                                {
                                                    if (j == 0)
                                                    {
                                                        line = tile1Code + "-" + tile2Code;
                                                        //line = tile1Code.Substring(0, 1) + "xx-" + tile2Code.Substring(0, 1) + "xx";
                                                    }
                                                    else
                                                    {
                                                        line = line + ";" + tile1Code + "-" + tile2Code;
                                                        //line= line+";"+ tile1Code.Substring(0, 1) + "xx-" + tile2Code.Substring(0, 1) + "xx";
                                                    }
                                                }
                                                else
                                                    if (tile2Code.Substring(0, 1) == "b" && (tile1Code.Substring(0, 1) == "s" || tile1Code.Substring(0, 1) == "f"))
                                                {
                                                    if (j == 0)
                                                    {
                                                        line = tile2Code + "-" + tile1Code;
                                                        //line = tile2Code.Substring(0, 1) + "xx-" + tile1Code.Substring(0, 1) + "xx";
                                                    }
                                                    else
                                                    {
                                                        line = line + ";" + tile2Code + "-" + tile1Code;
                                                        //line= line+";"+ tile2Code.Substring(0, 1) + "xx-" + tile1Code.Substring(0, 1) + "xx";
                                                    }
                                                }
                                                else
                                                    if (tile1Code.Substring(0, 1) == "s")//se infiere que la otra es f porque es la unica posibilidad
                                                {
                                                    if (j == 0)
                                                    {
                                                        line = tile1Code + "-" + tile2Code;
                                                        //line = tile1Code.Substring(0, 1) + "xx-" + tile2Code.Substring(0, 1) + "xx";
                                                    }
                                                    else
                                                    {
                                                        line = line + ";" + tile1Code + "-" + tile2Code;
                                                        //line= line+";"+ tile1Code.Substring(0, 1) + "xx-" + tile2Code.Substring(0, 1) + "xx";
                                                    }
                                                }
                                                else
                                                    if (tile2Code.Substring(0, 1) == "s")//Se infiere que la otra es f
                                                {
                                                    if (j == 0)
                                                    {
                                                        line = tile2Code + "-" + tile1Code;
                                                        //line= tile2Code.Substring(0, 1) + "xx-" + tile1Code.Substring(0, 1) + "xx";
                                                    }
                                                    else
                                                    {
                                                        line = line + ";" + tile2Code + "-" + tile1Code;
                                                        //line= line+";"+ tile2Code.Substring(0, 1) + "xx-" + tile1Code.Substring(0, 1) + "xx";
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.Log("Error, condicion no validada");
                                                    Message.showMessageText("Error, condicion no validada");
                                                }

                                            }
                                            else//Dind't found the posible second Tile
                                            {
                                                if (j == 0)
                                                {
                                                    line = TileCodification.getTileCode(tilesPlaced[k].TileName) + "-0";//Codigo del tile
                                                    //line = "xxx-0";
                                                }
                                                else
                                                {
                                                    line = line + ";" + TileCodification.getTileCode(tilesPlaced[k].TileName) + "-0";
                                                    //line = line + ";xxx-0";
                                                }
                                            }
                                        }
                                        else//Found a Front Tile
                                        {
                                            if (j == 0)
                                            {
                                                line = "0-" + TileCodification.getTileCode(tilesPlaced[k].TileName);//Codigo del tile
                                                //line = "0-xxx";
                                            }
                                            else
                                            {
                                                line = line + ";0-" + TileCodification.getTileCode(tilesPlaced[k].TileName);
                                                //line = line + ";0-xxx";
                                            }

                                        }


                                    }
                                    else//There is no tile there, so it's 0
                                    {
                                        if (j == 0)
                                        {
                                            line = "0-0";
                                        }
                                        else
                                        {
                                            line = line + ";0-0";
                                        }
                                    }
                                }
                                //Debug.Log(line);
                                sw.WriteLine(line);
                            }
                            sw.Close();
                            Message.showMessageText("World Saved Succesfully", MessageType.OK);
                        }
                        catch (Exception e)
                        {
                            Message.showMessageText(e.ToString());
                        }
                    }
                else
                {
                    Message.showMessageText("There is no spawn point on the world, must be at least one", MessageType.Error,5);
                }
            }
                else
                {
                    Message.showMessageText("This World Already Exists, Change the Name", MessageType.Error,3);
                }
            }
            else
            {
                Message.showMessageText("Invalid Name", MessageType.Error);
            }
        }
        else
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (!Directory.Exists("/storage/emulated/0/SquadCraft"))//If there is no SquadcraftBattles folder on sd (Usually when first installed)
                {
                    Directory.CreateDirectory("/storage/emulated/0/SquadCraft");//Creates the folder
                }
#elif UNITY_EDITOR
                //On the editor the folder "always exist", dont erase anaything, I create them manually
#endif
                Directory.CreateDirectory(worldsFilesPath);//Creates the My worlds where worlds are storage
            }
            catch (Exception e)
            {
                Message.showMessageText(e.ToString());
            }
            saveWorld(tilesPlaced, sizeX, sizeY, worldName);
        }
    }

    /// <summary>
    /// To delete an existing world.
    /// </summary>
    /// <param name="worldFile"> the world file name (string) you wants to delete</param>
    public static void deleteWorld(string worldFile)
    {
        try
        {
            File.Delete(worldsFilesPath + "/" + worldFile);
            Message.showMessageText("World Deleted Succesfully", MessageType.OK,5);
        }
        catch (Exception e)
        {
            Message.showMessageText(e.ToString());
        }
        //playMenuScript.UpdateWorlds = true;
    }

    /// <summary>
    /// To verify that at least there is one spawn point on the world.
    /// </summary>
    /// <param name="tilesList"> the list of tiles placed.</param>
    /// <returns>True if there is at least one spawn point, false if not.</returns>
    private static bool verifHasSpawnPoints(List<mapEditorMenuScript.TileInWorldSpace> tilesList)
    {
        if (tilesList.Count != 0)
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                if (tilesList[i].TileName.Equals("Spawn Point"))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
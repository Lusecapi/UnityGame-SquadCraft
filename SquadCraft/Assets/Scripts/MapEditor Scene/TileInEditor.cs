using UnityEngine;
using UnityEngine.EventSystems;

public class TileInEditor : MonoBehaviour, IPointerClickHandler {

    public bool canPlaceOverIt = true;

    public void OnPointerClick(PointerEventData eventData)
    {

        if (mapEditorMenuScript.isEraserSelected)
        {
            bool sw = false;
            mapEditorMenuScript.wasUndo = false;
            mapEditorMenuScript.lastEditAction = EditAction.Erase;
            for (int i = mapEditorMenuScript.tilesPlaced.Count-1; i >= 0; i--)//Toca buscarlo en Reversa para borrar las posibles tiles que esten sobre otras, las cuales fueron colocadas de ultimo
            {
                mapEditorMenuScript.TileInWorldSpace tile = mapEditorMenuScript.tilesPlaced[i];
                if (tile.Position.Equals(transform.position) && !sw)
                {
                    //Delete the Object and removing from the list
                    mapEditorMenuScript.lastDeletedTiles.Insert(0, tile);
                    mapEditorMenuScript.tilesPlaced.Remove(tile);
                    Destroy(tile.TileGameObject);
                    sw = true;
                }
            }
            if (mapEditorMenuScript.tilesPlaced.Count == 0)
            {
                mapEditorMenuScript.wasClearAll = true;
            }
            else
            {
                mapEditorMenuScript.wasClearAll = false;
            }
        }
        else
            if (mapEditorMenuScript.isPencilSelected)
        {
            if (canPlaceOverIt)
            {
                if (gameObject.layer == LayerMask.NameToLayer("Background Tile"))
                {

                    GameObject tilePrefab = Resources.Load("Tiles/" + mapEditorMenuScript.targetTileName) as GameObject;
                    if (tilePrefab.layer != LayerMask.NameToLayer("Background Tile"))
                    {
                        instantiateTile();
                    }

                }
                else
                    if(gameObject.layer == LayerMask.NameToLayer("Special Tile"))
                {
                    GameObject tilePrefab = Resources.Load("Tiles/" + mapEditorMenuScript.targetTileName) as GameObject;
                    if (tilePrefab.layer == LayerMask.NameToLayer("Front Tile"))
                    {
                        instantiateTile();
                    }
                }
            }
        }
    }

    private void instantiateTile()
    {

        mapEditorMenuScript.wasUndo = false;
        mapEditorMenuScript.wasClearAll = false;
        mapEditorMenuScript.lastEditAction = EditAction.Draw;
        GameObject tile = Instantiate(Resources.Load("Tiles/" + mapEditorMenuScript.targetTileName), transform.position, Quaternion.identity) as GameObject;
        tile.tag = "Tile in Editor";
        //We Remove all the Components, beacuse we are gonna use as Editor tile, and not In Normal game

        Destroy(tile.GetComponent<Tile>());
        Destroy(tile.GetComponent<BoxCollider2D>());
        Destroy(tile.GetComponent<BoxCollider2D>());

        //Now We Add the Components We really Need
        tile.AddComponent<BoxCollider2D>();
        tile.AddComponent<TileInEditor>();
        tile.GetComponent<TileInEditor>().canPlaceOverIt = false;
        mapEditorMenuScript.tilesPlaced.Add(new mapEditorMenuScript.TileInWorldSpace(mapEditorMenuScript.targetTileName, transform.position, tile));

    }

}

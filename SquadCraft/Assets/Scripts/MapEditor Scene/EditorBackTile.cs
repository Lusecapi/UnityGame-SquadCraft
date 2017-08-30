using UnityEngine;
using UnityEngine.EventSystems;

public class EditorBackTile : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mapEditorMenuScript.isPencilSelected)
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
            mapEditorMenuScript.tilesPlaced.Add(new mapEditorMenuScript.TileInWorldSpace(mapEditorMenuScript.targetTileName, transform.position, tile));
        }
    }
}

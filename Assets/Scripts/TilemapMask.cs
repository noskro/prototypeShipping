using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMask : MonoBehaviour
{
    //-- Made by @JustAnCore
    //-- License: Apache 2.0

    public GameObject maskCell;
    public CustomTile tileToMask;

    private List<GameObject> masks = new List<GameObject>();

    private void Start()
    {
        DoIt();
    }

    public void DoIt()
    { 
        Tilemap tilemap = GetComponent<Tilemap>();

        Vector3Int startCoord = tilemap.origin;
        Vector3Int size = tilemap.size;

        foreach(GameObject go in masks)
        {
            Destroy(go);
        }

        masks = new List<GameObject>();

        //Iterate over each cell
        for (int x = startCoord.x; x < startCoord.x + size.x; x++)
        {
            for (int y = startCoord.y; y < startCoord.y + size.y; y++)
            {
                //Check if cell isn't empty
                if (tilemap.GetTile(new Vector3Int(x, y, startCoord.z)) != null && tilemap.GetTile<CustomTile>(new Vector3Int(x, y, startCoord.z)).Equals(tileToMask))
                {
                    //Create maskCell on the cell coords
                    Vector3 coord = tilemap.CellToWorld(new Vector3Int(x, y, startCoord.z));
                    masks.Add(Instantiate(maskCell, coord, Quaternion.identity, transform));
                }
            }
        }
    }
}

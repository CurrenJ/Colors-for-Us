using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int tileSize;
    public float tileXOffset;
    public float tileYOffset;
    private Tile[,] tiles = new Tile[500, 500];
    public List<Tile> dirtyTiles = new List<Tile>();
    public GameObject player;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tIndex = locToTileIndex(player.transform.position);
        Tile tile = getTile(new Vector2(tIndex.x, 4));
        if(!tile.isFlashing() && !tile.flashed)
            tile.flash(0.5F, 1F);
       

        for (int d = 0; d < dirtyTiles.Count; d++) {
            dirtyTiles[d].tileUpdate();
        }
    }

    public void addTile(Tile tile)
    {
        Vector2 loc = locToTileIndex(new Vector2(tile.transform.position.x, tile.transform.position.y));
        Debug.Log("Adding tile at " + loc);
        tiles[(int)loc.x, (int)loc.y] = tile;
    }

    public Vector2 tileIndexToLoc(Vector2 tileIndex) {
        return new Vector2(tileIndex.x * tileSize + tileXOffset, tileIndex.y * tileSize + tileYOffset);
    }

    public Vector2 locToTileIndex(Vector2 loc) {
        return new Vector2(Mathf.Floor((loc.x - tileXOffset + tileSize / 2.0F) / tileSize), Mathf.Floor((loc.y - tileYOffset + tileSize / 2.0F) / tileSize));
    }

    public Tile getTile(Vector2 indices) {
        return tiles[(int)indices.x, (int)indices.y];
    }

    public void markDirty(Tile tile) {
        if(!dirtyTiles.Contains(tile))
            dirtyTiles.Add(tile);
    }

    public void markClean(Tile tile) {
        dirtyTiles.Remove(tile);
    }

    public void flashBelow(Tile tile) {
        Vector2 indices = locToTileIndex(tile.transform.position);
        Debug.Log(indices);
        if (indices.y - 1 > 0 && indices.y - 1 < tiles.GetLength(1)) {
            tiles[(int)indices.x, (int)indices.y - 1].flash(tile.hue, tile.peakSaturation - 0.25F);
        }
    }
}

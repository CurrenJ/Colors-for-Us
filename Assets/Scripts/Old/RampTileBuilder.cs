using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampTileBuilder : MonoBehaviour
{
    public int tileSize;
    public int xSize;
    public int ySize;
    public GameController controller;
    public GameObject tilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        placeTiles();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void placeTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.SetParent(this.transform);
                newTile.transform.localPosition = new Vector3(tileSize * x, tileSize * y, this.transform.position.z);
                newTile.GetComponent<Tile>().addTileToController(controller);
                Destroy(newTile.GetComponent<BoxCollider2D>());
            }
        }
        BoxCollider2D collider = this.gameObject.AddComponent<BoxCollider2D>();
        collider.offset = new Vector2(xSize * tileSize / 2, ySize * tileSize / 2);
        collider.size = new Vector2(xSize * tileSize, ySize * tileSize);
    }
}

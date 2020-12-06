using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampBuilder : MonoBehaviour
{
    public float angle;
    public bool platformAngleOverride;
    public float platformDataAngleOverride;
    public float lengthX;
    public PlatformController controller;
    public Material material;
    public GameObject supportBox;
    public GameObject floorParent;
    public bool saveAsAsset;
    public bool genWater;
    public GameObject waterFlow;
    public Sprite maskSprite;

    // Start is called before the first frame update
    void Start()
    {
        angle = Mathf.Deg2Rad * angle;
        for (int i = 0; i < lengthX; i++)
        {
            createRampPiece(i);
        }

        createEdge();

        if (saveAsAsset)
            saveAsset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void saveAsset()
    {
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(GetComponentInChildren<MeshFilter>().mesh, "Assets/Tile Prefabs/ButtonRamp.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }

    private void createEdge() {
        for (int f = 0; f < 2; f++)
        {
            GameObject gameObject = new GameObject("Ramp Floor");
            gameObject.transform.SetParent(floorParent.transform);
            float actualLength = ((lengthX) * controller.tileSize) / Mathf.Cos(angle);
            float y = ((Mathf.Tan(angle) * controller.tileSize * lengthX)) / 2;

            gameObject.transform.position = this.transform.position + Vector3.Scale(new Vector3(controller.tileSize * lengthX / 2 - controller.tileSize / 2, y - controller.tileSize / 2, 0), this.transform.lossyScale);
            gameObject.transform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
            gameObject.transform.localScale = Vector3.Scale(new Vector3(actualLength, 1, 1), this.transform.lossyScale);
            gameObject.AddComponent<EdgeCollider2D>();
            if (gameObject.transform.lossyScale.x < 0)
            {
                Vector3 rotation = gameObject.transform.localEulerAngles;
                rotation.z *= -1;
                gameObject.transform.localEulerAngles = rotation;
            }

            if(f == 0)
                gameObject.layer = LayerMask.NameToLayer("Edges");
            else if(f == 1)
                gameObject.layer = LayerMask.NameToLayer("Interactable Visible Edges");

            gameObject.tag = "Floor";
        }
    }

    public void createRampPiece(int x) {
        Vector3[] vertices = new Vector3[5];
        Vector2[] uv = new Vector2[5];
        int[] triangles = new int[9];

        float deltaY = Mathf.Tan(angle) * controller.tileSize;
        float startX = -controller.tileSize / 2 + x * controller.tileSize;
        float startY = -controller.tileSize / 2 + deltaY * x;

        //slant
        vertices[0] = new Vector3(startX, startY);
        vertices[1] = new Vector3(startX + controller.tileSize, startY + deltaY);
        vertices[2] = new Vector3(startX + controller.tileSize, startY);
        //base
        vertices[3] = new Vector3(startX + controller.tileSize, startY < controller.tileSize / 2 ? -controller.tileSize / 2 : startY - ((startY - controller.tileSize / 2) % controller.tileSize));
        vertices[4] = new Vector3(startX, startY < controller.tileSize / 2 ? -controller.tileSize / 2 : startY - ((startY - controller.tileSize / 2) % controller.tileSize));
    
        uv[0] = new Vector2(startX, startY);
        uv[1] = new Vector2(startX + controller.tileSize, startY + deltaY);
        uv[2] = new Vector2(startX + controller.tileSize, startY);
        uv[3] = new Vector3(startX + controller.tileSize, startY < controller.tileSize / 2 ? -controller.tileSize / 2 : startY - ((startY - controller.tileSize / 2) % controller.tileSize));
        uv[4] = new Vector3(startX, startY < controller.tileSize / 2 ? -controller.tileSize / 2 : startY - ((startY - controller.tileSize / 2) % controller.tileSize));

        int supportBoxes = (int)(startY < controller.tileSize / 2 ? 0 : (startY + controller.tileSize / 2) / controller.tileSize);
        generateSupportBoxes(x, supportBoxes);

        if (genWater)
            generateWaterFlow(0.5F, startX, startY, x == lengthX - 1);

        //slant
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        //base top-left
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 4;
        //base bottom-right
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;


        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };

        GameObject meshGO = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D), typeof(Platform));
        meshGO.transform.SetParent(this.transform);
        meshGO.transform.localPosition = new Vector3(0, 0, 0);
        meshGO.transform.localScale = new Vector3(1, 1, 1);
        meshGO.GetComponent<MeshFilter>().mesh = mesh;
        meshGO.GetComponent<MeshRenderer>().material = material;
        meshGO.GetComponent<MeshRenderer>().material.mainTexture = SpriteEffects.createTexture(Color.white);
        meshGO.GetComponent<Platform>().controller = controller;
        if(!platformAngleOverride)
            meshGO.GetComponent<Platform>().setAngle(angle);
        else meshGO.GetComponent<Platform>().setAngle(platformDataAngleOverride * Mathf.Deg2Rad);
        meshGO.GetComponent<PolygonCollider2D>().SetPath(0, uv);
        meshGO.GetComponent<PolygonCollider2D>().offset = new Vector2(0, -0.02F);
        meshGO.GetComponent<MeshRenderer>().sortingLayerID = SortingLayer.NameToID("Foreground");
        meshGO.layer = this.gameObject.layer;
    }

    private void generateSupportBoxes(int x, int num) {
        for (int b = 0; b < num; b++)
        {
            GameObject sbGO = Instantiate(supportBox);
            sbGO.transform.SetParent(this.transform);
            sbGO.GetComponent<Platform>().controller = controller;
            sbGO.transform.localPosition = new Vector3(x * controller.tileSize, b * controller.tileSize, 0);
            sbGO.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Foreground");
            sbGO.layer = LayerMask.NameToLayer("Platforms For Pulse");
        }
    }

    private void generateWaterFlow(float scale, float startX, float startY, bool lastPiece) {
        float deltaY = Mathf.Tan(angle) * controller.tileSize;
        for (int w = 0; w < 1 / scale; w++)
        {
            GameObject maskGO = new GameObject();
            maskGO.transform.SetParent(this.transform);
            SpriteMask mask = maskGO.AddComponent<SpriteMask>();
            mask.sprite = maskSprite;
            maskGO.transform.localPosition = new Vector3(startX + controller.tileSize * scale * w + scale / 2, startY + scale * deltaY * w + controller.tileSize / 2 * scale, 0);
            maskGO.transform.eulerAngles = new Vector3(0, 0, 0);
            maskGO.transform.localScale = new Vector3(scale, scale, scale);
            maskGO.name = "Mask";

            float hypot = controller.tileSize / Mathf.Cos(angle);
            float heightScale = Mathf.Cos(angle) / controller.tileSize;
            GameObject flow = Instantiate(waterFlow);
            flow.transform.SetParent(maskGO.transform);
            flow.transform.localPosition = new Vector2(-scale, -scale);
            flow.transform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
            flow.transform.localScale = new Vector3(1 * hypot, 1 * heightScale, 1);
            flow.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            flow.name = "Water Flow";
            flow.GetComponent<RippleOffset>().offset.x = (startX * 2 + w) * 0.045F;
            flow.GetComponent<SpriteRenderer>().sortingOrder = 1;

            if (lastPiece && w == 1 / scale - 1)
            {
                GameObject flow2 = Instantiate(waterFlow);
                flow2.transform.SetParent(maskGO.transform);
                flow2.transform.localPosition = new Vector2(scale, -scale + deltaY * scale * 2);
                flow2.transform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
                flow2.transform.localScale = new Vector3(1 * hypot, 1 * heightScale, 1);
                flow2.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                flow2.name = "Water Flow Ramp End";
                flow2.GetComponent<RippleOffset>().offset.x = (startX * 2 + w) * 0.045F;
                flow2.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        } 
    }
}

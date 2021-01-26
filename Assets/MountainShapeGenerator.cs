using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.Rendering.Universal;
using System;

public class MountainShapeGenerator : MonoBehaviour
{
    public bool fromLineRenderer;
    public Material material;
    public Color colorTop;
    public Color colorBase;
    [Range(0F, 1F)]
    public float peakGradientFactor;
    [Range(0, 500)]
    public int peakRadiusPixels;

    [Range(1, 30)]
    public int pointsToAdd;
    [Range(-10F, 10F)]
    public float varianceMin;
    [Range(-10F, 10F)]
    public float varianceMax;
    private float rateOfChange;
    public bool debugPoints;
    public bool debugRateOfChange;
    private Vector3[] original;
    // Start is called before the first frame update
    void Start()
    {
        savePoints();
        generateMesh();
    }

    void OnValidate()
    {
        if (Application.isPlaying && original != null)
        {
            generateMesh();
        }
    }

    private void savePoints()
    {
        if (fromLineRenderer)
        {
            original = new Vector3[GetComponent<LineRenderer>().positionCount];
            Debug.Log(GetComponent<LineRenderer>().GetPositions(original));
            Destroy(GetComponent<LineRenderer>());
        }
    }

    private void generateMesh()
    {

        Vector2[] vertices2D = new Vector2[original.Length + pointsToAdd * (original.Length - 1)];
        if (fromLineRenderer)
        {
            int points = original.Length;
            for (int p = 0; p < points - 1; p++)
            {
                rateOfChange = 0;
                Vector3 pointA = original[p];
                Vector3 pointB = original[p + 1];
                vertices2D[p + p * pointsToAdd] = pointA;
                vertices2D[p + ((p + 1) * pointsToAdd) + 1] = pointB;
                for (int n = 0; n < pointsToAdd; n++)
                {
                    Vector3 pointC = Vector3.Lerp(pointA, pointB, (1F + n) / (pointsToAdd + 1F));
                    rateOfChange += RandomGaussian(varianceMin / pointsToAdd, varianceMax / pointsToAdd);
                    pointC += new Vector3(0, rateOfChange * (Mathf.Abs(pointB.y - pointA.y) / 2), 0);
                    vertices2D[p + p * pointsToAdd + n + 1] = pointC;

                    if (debugRateOfChange)
                        Debug.Log(rateOfChange);

                    if (debugPoints)
                        Debug.Log("P[" + (p + (p * pointsToAdd)) + "] " + pointA.x + ", " + pointA.y + "-" + "P[" + (p + 1 + (p * pointsToAdd)) + "] " + pointB.x + ", " + pointB.y + " + P[" + (p + (p * pointsToAdd) + n) + "] " + pointC.x + ", " + pointC.y);
                }

            }
        }

        Vector2 yBounds = new Vector2(0, 0);
        Vector2 xBounds = new Vector2(0, 0);
        int xMinIndex = 0;
        int xMaxIndex = 0;
        // Create the Vector3 vertices
        // Insert new points between existing ones.
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                yBounds = new Vector2(vertices2D[i].y, vertices2D[i].y);
                xBounds = new Vector2(vertices2D[i].x, vertices2D[i].x);
            }
            else
            {
                if (vertices2D[i].y > yBounds.y)
                    yBounds = new Vector2(yBounds.x, vertices2D[i].y);
                else if (vertices2D[i].y < yBounds.x)
                {
                    yBounds = new Vector2(vertices2D[i].y, yBounds.y);
                }

                if (vertices2D[i].x > xBounds.y)
                {
                    xBounds = new Vector2(xBounds.x, vertices2D[i].x);
                    xMaxIndex = i;
                }
                else if (vertices2D[i].x < xBounds.x)
                {
                    xBounds = new Vector2(vertices2D[i].x, xBounds.y);
                    xMinIndex = i;
                }
            }
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        //populate basic UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i2 = 0; i2 < vertices.Length; i2++)
        {
            uvs[i2] = new Vector2((vertices[i2].x - xBounds.x) / (xBounds.y - xBounds.x), (vertices[i2].y - yBounds.x) / (yBounds.y - yBounds.x));
        }

        vertices2D = new Vector2[vertices.Length];
        for (int x = 0; x < vertices.Length; x++)
        {
            vertices2D[x] = vertices[x];
        }
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();
        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.uv = uvs;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        MeshRenderer meshRenderer;
        if (!TryGetComponent<MeshRenderer>(out meshRenderer))
            meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter;
        if (!TryGetComponent<MeshFilter>(out filter))
            filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = material;
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", generateTexture2d(vertices, xBounds, yBounds));
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_GradientFactor", peakGradientFactor);
        Destroy(GetComponent<SpriteShapeController>());
        Destroy(GetComponent<SpriteShapeRenderer>());

        ShadowCaster2D shadowCaster;
        if(TryGetComponent<ShadowCaster2D>(out shadowCaster)){
            //shadowCaster.setRenderMesh(vertices);
        }

        meshRenderer.material.SetColor("_ColorB", colorTop);
        meshRenderer.material.SetColor("_ColorA", colorBase);

    }

    private Texture2D generateTexture2d(Vector3[] vertices, Vector2 xBounds, Vector2 yBounds)
    {
        float textureScale = 100;
        Texture2D tex = new Texture2D((int)((xBounds.y - xBounds.x) * textureScale), (int)((yBounds.y - yBounds.x) * textureScale));
        tex.Apply();

        Color[] baseColorArr = new Color[tex.width * tex.height];
        for (int p = 0; p < tex.width * tex.height; p++)
        {
            baseColorArr[p] = colorBase;
        }
        tex.SetPixels(0, 0, tex.width, tex.height, baseColorArr);

        Vector3[] deltas = new Vector3[vertices.Length];
        ArrayList peakDeltasOnly = new ArrayList();
        for (int p = 0; p < vertices.Length; p++)
        {
            float delta;
            if (p == 0)
                delta = vertices[p].y - vertices[vertices.Length - 1].y;
            else delta = vertices[p].y - vertices[p - 1].y;

            deltas[p] = new Vector3(vertices[p].x, vertices[p].y, delta);
        }

        for (int d = 0; d < vertices.Length; d++)
        {
            if (d == 0)
            {
                deltas[d] = new Vector3(deltas[d].x, deltas[d].y, 0);
            }
            else
            {
                if (deltas[d].z < 0 && deltas[d - 1].z >= 0 && (deltas[d - 1].y >= yBounds.x + 0.25F * (yBounds.y - yBounds.x)))
                {
                    deltas[d - 1] = new Vector3(deltas[d - 1].x, deltas[d - 1].y, 1);
                    peakDeltasOnly.Add(deltas[d - 1]);
                }
            }
        }


        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Vector2 pixelPos = new Vector2(x, y);
                float minDistance = peakRadiusPixels;
                for (int d = 0; d < peakDeltasOnly.Count; d++)
                {
                    Vector3 delt = (((Vector3)peakDeltasOnly[d]));
                    Vector2 peakPos = new Vector2(((int)((delt.x - xBounds.x) * textureScale)), ((int)((delt.y - yBounds.x) * textureScale)));
                    float distance = Vector2.Distance(pixelPos, peakPos);
                    if (distance <= peakRadiusPixels)
                    {
                        if (d == 0 || distance < minDistance)
                            minDistance = distance;
                    }
                }
                Color pixelColor = Color.Lerp(colorTop, colorBase, minDistance / peakRadiusPixels);
                tex.SetPixel((int)pixelPos.x, (int)pixelPos.y, pixelColor);
            }
        }
        tex.Apply();


        return tex;
    }

    private Vector2[] generateUVs(Vector3[] vertices, Vector2 yBounds)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        Vector3[] deltas = new Vector3[vertices.Length];
        ArrayList peakDeltasOnly = new ArrayList();
        for (int p = 0; p < vertices.Length; p++)
        {
            float delta;
            if (p == 0)
                delta = vertices[p].y - vertices[vertices.Length - 1].y;
            else delta = vertices[p].y - vertices[p - 1].y;

            deltas[p] = new Vector3(vertices[p].x, vertices[p].y, delta);
        }

        for (int d = 0; d < vertices.Length; d++)
        {
            if (d == 0)
            {
                // if (deltas[d].z < 0 && deltas[vertices.Length - 1].z >= 0)
                // {
                //     deltas[d] = new Vector3(deltas[d].x, deltas[d - 1].y, 1);
                //     deltasOnly.Add(deltas[d]);
                //     uvs[d] = new Vector2(0, 1);
                // }
                // else uvs[d] = new Vector2(0, 0);
                deltas[d] = new Vector3(deltas[d].x, deltas[d].y, 0);
            }
            else
            {
                if (deltas[d].z < 0 && deltas[d - 1].z >= 0 && (deltas[d - 1].y >= yBounds.x + 0.25F * (yBounds.y - yBounds.x)))
                {
                    deltas[d - 1] = new Vector3(deltas[d - 1].x, deltas[d - 1].y, 1);
                    peakDeltasOnly.Add(deltas[d - 1]);
                    uvs[d - 1] = new Vector2(0, 1);
                }
                else uvs[d] = new Vector2(0, 0);
            }
        }

        for (int d = 0; d < peakDeltasOnly.Count; d++)
        {
            Vector3 delt = (((Vector3)peakDeltasOnly[d]));
            Debug.DrawRay(this.transform.position + new Vector3(delt.x, delt.y, 0), transform.up * 1F, Color.red, 1f, false);
        }

        for (int v = 0; v < vertices.Length; v++)
        {
            Vector3 closestDelta = new Vector3();
            for (int d = 0; d < peakDeltasOnly.Count; d++)
            {
                Vector3 delt = (((Vector3)peakDeltasOnly[d]));
                if (d == 0)
                {
                    closestDelta = delt;
                }
                else if (Vector3.Distance(delt, vertices[v]) < Vector3.Distance(closestDelta, vertices[v]))
                {
                    closestDelta = delt;
                }
            }
            if (vertices[v].y < (yBounds.y - yBounds.x) * 0.1F + yBounds.x)
            {
                uvs[v] = new Vector2((vertices[v].y - yBounds.x) / (yBounds.y - yBounds.x), 0);
            }
            else if (Vector3.Distance(closestDelta, vertices[v]) == 0)
            {
                uvs[v] = new Vector2(0, 1);
            }
            else
            {
                uvs[v] = new Vector2(0, 0.5F / Vector3.Distance(closestDelta, vertices[v]));
            }

            Debug.DrawRay(this.transform.position + new Vector3(vertices[v].x, vertices[v].y, 0), transform.up * 0.5F, new Color(uvs[v].y, uvs[v].y, uvs[v].y, 1), 1f, false);
            Debug.DrawRay(this.transform.position + new Vector3(vertices[v].x, vertices[v].y, 0), transform.right * 0.2F, new Color(uvs[v].y, uvs[v].y, uvs[v].y, 1), 1f, false);
            Debug.DrawRay(this.transform.position + new Vector3(vertices[v].x, vertices[v].y, 0), transform.right * -0.2F, new Color(uvs[v].y, uvs[v].y, uvs[v].y, 1), 1f, false);
        }

        return uvs;
    }

    //@Oneiros90
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
}

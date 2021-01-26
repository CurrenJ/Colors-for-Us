using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;

public class SpriteShapeShaderFix : MonoBehaviour
{
    public Color colorA;
    public Color colorB;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetColor("_ColorA", colorA);
        GetComponent<Renderer>().material.SetColor("_ColorB", colorB);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleOffset : MonoBehaviour
{
    public Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetVector("_RippleOffset", new Vector4(offset.x, offset.y, 0, 0));
        GetComponent<SpriteRenderer>().material = mat;
;    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

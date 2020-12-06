using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBuilder : MonoBehaviour
{
    public float coverage;
    public Vector2 size;
    public GameObject vines;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                if (Random.Range(0F, 1F) < coverage)
                {
                    GameObject v = Instantiate(vines);
                    v.transform.SetParent(this.transform);
                    v.transform.localPosition = new Vector2(x, y);
                    v.GetComponent<Renderer>().material.SetVector("_Offset", v.transform.localPosition);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

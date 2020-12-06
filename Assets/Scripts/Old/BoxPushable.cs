using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPushable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onCollisionEnter2D(Collision2D col) {
        Debug.Log(col.transform.name);
    }

    public void onCollisionExit2D(Collision2D col)
    {
    }

    public void onCollisionStay2D(Collision2D col)
    {
        Debug.Log(col.transform.name);
    }
}

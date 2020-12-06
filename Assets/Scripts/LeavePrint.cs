using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavePrint : MonoBehaviour
{
    public Material glowMat;
    public float startDelay;
    public float printSpacingTime;
    private float lastPrintTime;
    public Vector2 printFadeTime;
    public List<GameObject> prints;

    // Start is called before the first frame update
    void Start()
    {
        prints = new List<GameObject>();
        lastPrintTime = startDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > lastPrintTime) {
            leavePrint();
            lastPrintTime += printSpacingTime;
        }

        for(int p = 0; p < prints.Count; p++) {
            GameObject g = prints[p];
            if (prints[p].GetComponent<SpriteRenderer>().color.a == 0)
            {
                Destroy(g);
                prints.Remove(g);
                p--;
            }
        }
    }

    private void leavePrint() {
        GameObject print = new GameObject();
        SpriteRenderer _Renderer = print.AddComponent<SpriteRenderer>();
        _Renderer.material = glowMat;
        _Renderer.sortingLayerName = "Foreground";
        _Renderer.sprite = this.GetComponent<SpriteRenderer>().sprite;
        print.transform.position = this.transform.position;
        print.transform.rotation = this.transform.rotation;
        Color col;
        ColorUtility.TryParseHtmlString("#48dbc3", out col);
        col.a = 0.5F;
        print.GetComponent<SpriteRenderer>().color = col;


        MoveSwitch mS = print.AddComponent<MoveSwitch>();
        mS.autoStart = true;
        mS.offset = new Vector2(Random.Range(-0.3F, 0.3F), Random.Range(0, -0.6F));
        mS.time = Random.Range(printFadeTime.x, printFadeTime.y);

        RotationSwitch rS = print.AddComponent<RotationSwitch>();
        rS.autoStart = true;
        rS.rotationChange = Random.Range(-15F, 15F);
        rS.time = Random.Range(printFadeTime.x, printFadeTime.y);

        OpacitySwitch oS = print.AddComponent<OpacitySwitch>();
        oS.autoStart = true;
        oS.opacity = new Vector2(_Renderer.color.a, 0);
        oS.time = Random.Range(printFadeTime.x, printFadeTime.y);

        prints.Add(print);
    }
}

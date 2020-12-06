using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBuilder : MonoBehaviour
{
    [Range(2, 20)]
    public int heightMin;
    [Range(2, 20)]
    public int heightMax;
    [Range(0.0F, 1.0F)]
    public float damage;
    [Range(0.0F, 1.0F)]
    public float depth;
    public List<Sprite> bottoms;
    public List<Sprite> tops;
    public List<Sprite> middles;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        generatePillar(heightMin + Random.Range(0, heightMax - heightMin + 1), damage, depth);
    }

    public void generatePillar(int h, float dmg, float dpth) {
        for (int p = 0; p < h; p++) {
            generatePillarSection(p, h, dmg, dpth);
        }
    }

    public void generatePillarSection(int h, int maxH, float dmg, float dpth) {
        Sprite sprite;
        Vector2 localP = new Vector2(0, 0);
        Vector3 localRotation = new Vector3(0, 0, 0);
        Vector3 localScale = new Vector3(1, 1, 1);

        float damageSeed = Random.Range(0.0F, 1.0F);
        if (h == 0)
        {
            if (damageSeed < dmg)
                sprite = bottoms[Random.Range(1, bottoms.Count)];
            else sprite = bottoms[0];
        }
        else if (h == maxH - 1)
        {
            if (damageSeed < dmg)
                sprite = tops[Random.Range(1, tops.Count)];
            else sprite = tops[0];
        }
        else
        {
            if (damageSeed < dmg)
                sprite = middles[Random.Range(1, middles.Count)];
            else sprite = middles[0];
        }

        if (h == maxH - 1)
        {
            localScale.y *= -1;
        }
        else if(h != 0)
        {
            if (Random.Range(0, 2) == 1)
                localScale.y *= -1;
        }

        localP += new Vector2(0, h);

        GameObject pillarGO = new GameObject();
        pillarGO.transform.SetParent(this.transform);
        pillarGO.transform.localPosition = localP;
        pillarGO.transform.localEulerAngles = localRotation;
        pillarGO.transform.localScale = localScale;
        pillarGO.AddComponent<SpriteRenderer>();
        pillarGO.GetComponent<SpriteRenderer>().sprite = sprite;
        Color.RGBToHSV(pillarGO.GetComponent<SpriteRenderer>().color, out float hue, out float sat, out float v);
        v = 1 - dpth;
        pillarGO.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue, sat, v);
        pillarGO.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Background Near");
        pillarGO.GetComponent<SpriteRenderer>().material = mat;
    }
}

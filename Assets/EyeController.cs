using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EyeController : MonoBehaviour
{
    public int numEyes;
    public List<GameObject> eyes;
    public GameObject eyePrefab;
    public float eyeSize;
    public Vector2 eyeSizeRange;
    public Material eyeMaterial;
    public Gradient irisColor;
    public bool startOpen;

    // Start is called before the first frame update
    void Start()
    {
        for (int k = 0; k < numEyes; k++)
            newRandomEye();

        for (int c = 0; c < eyes.Count; c++)
            Destroy(eyes[c].GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            blinkAll(true);
        if (Input.GetKeyDown(KeyCode.P))
            closeAll(true);
        if (Input.GetKeyDown(KeyCode.L))
            openAll(true);
    }

    private void newRandomEye() {
        GameObject newEye = Instantiate(eyePrefab);
        newEye.transform.SetParent(this.transform);
        newEye.name += " " + Random.Range(0, 100);
        newEye.transform.localScale *= Random.Range(eyeSizeRange.x, eyeSizeRange.y);
        Color iris = irisColor.Evaluate(Random.Range(0F, 1F));
        newEye.GetComponent<Renderer>().material = eyeMaterial;
        newEye.GetComponent<Renderer>().material.SetColor("_Color", iris);
        newEye.GetComponent<Animator>().speed = 4;

        Vector2[] colliderPath = newEye.GetComponent<PolygonCollider2D>().GetPath(0);
        float randomAngle = Random.Range(0F, 2 * Mathf.PI);
        float distanceOut = 0;
        bool overlapping = false;
        int attemptsFailsafe = 0;
        do
        {
            overlapping = false;
            newEye.transform.localPosition = getEyePos(randomAngle, distanceOut);
            DestroyImmediate(newEye.GetComponent<PolygonCollider2D>());
            newEye.AddComponent<PolygonCollider2D>().SetPath(0, colliderPath);

            for (int i = 0; i < eyes.Count && !overlapping; i++)
            {
                GameObject e = eyes[i];
                if (e.GetComponent<Collider2D>().bounds.Intersects(newEye.GetComponent<Collider2D>().bounds))
                {
                    overlapping = true;
                    distanceOut += eyeSize;
                }
            }

            attemptsFailsafe++;
        } while (overlapping && attemptsFailsafe < 100) ;


        if (!startOpen)
        {
            newEye.GetComponent<Animator>().Play("eye_closing", 0, 1F);
        }
        eyes.Add(newEye);
    }

    public void blinkOne() {
        blink(eyes[Random.Range(0, eyes.Count)]);
    }

    public void blinkAll(bool staggered) {
        foreach (GameObject gO in eyes)
        {
            if (staggered)
            {
                WaitSwitch wS = gO.AddComponent<WaitSwitch>();
                wS.autoStart = true;
                wS.time = Random.Range(0, 1F);
                wS.consequence = new List<EffectSwitch>();

                CustomEventSwitch cES = gO.AddComponent<CustomEventSwitch>();
                cES.enableEvents = new UnityEngine.Events.UnityEvent();
                cES.enableEvents.AddListener(delegate { blink(gO); });

                wS.consequence.Add(cES);
            }
            else
            {
                blink(gO);
            }
        }
    }

    public void closeAll(bool staggered) {
        foreach (GameObject gO in eyes) {
            if (staggered)
            {
                WaitSwitch wS = gO.AddComponent<WaitSwitch>();
                wS.autoStart = true;
                wS.time = Random.Range(0, 1F);
                wS.consequence = new List<EffectSwitch>();

                CustomEventSwitch cES = gO.AddComponent<CustomEventSwitch>();
                cES.enableEvents = new UnityEngine.Events.UnityEvent();
                cES.enableEvents.AddListener(delegate { close(gO); });

                wS.consequence.Add(cES);
            }
            else {
                close(gO);
            }
        }
    }

    public void openAll(bool staggered)
    {
        foreach (GameObject gO in eyes)
        {
            if (staggered)
            {
                WaitSwitch wS = gO.AddComponent<WaitSwitch>();
                wS.autoStart = true;
                wS.time = Random.Range(0, 1F);
                wS.consequence = new List<EffectSwitch>();

                CustomEventSwitch cES = gO.AddComponent<CustomEventSwitch>();
                cES.enableEvents = new UnityEngine.Events.UnityEvent();
                cES.enableEvents.AddListener(delegate { open(gO); });

                wS.consequence.Add(cES);
            }
            else
            {
                open(gO);
            }
        }
    }

    private Vector2 getEyePos(float angle, float dist) {
        return new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
    }

    private static void blink(GameObject eye) {
        resetTriggers(eye);
        eye.GetComponent<Animator>().SetTrigger("Blink");
    }

    private static void open(GameObject eye) {
        resetTriggers(eye);
        eye.GetComponent<Animator>().SetTrigger("Open");
    }

    private static void close(GameObject eye)
    {
        resetTriggers(eye);
        eye.GetComponent<Animator>().SetTrigger("Close");
    }

    private static void resetTriggers(GameObject eye) {
        eye.GetComponent<Animator>().ResetTrigger("Open");
        eye.GetComponent<Animator>().ResetTrigger("Close");
        eye.GetComponent<Animator>().ResetTrigger("Blink");
    }
}

using UnityEngine;
using System;

[Serializable]
public class ParallaxLayer
{
    public float distance;
    public Vector2[] startPositions;
    public GameObject[] gameObjects;
}

public class ParallaxController : MonoBehaviour
{
    public ParallaxLayer[] parallaxLayers;
    private Vector2 start;
    public bool lockYAxis;

    void Start()
    {
        this.start = this.transform.localPosition;
        foreach(ParallaxLayer layer in parallaxLayers) {
            layer.startPositions = new Vector2[layer.gameObjects.Length];
            for(int g = 0; g < layer.gameObjects.Length; g++){
                layer.startPositions[g] = layer.gameObjects[g].transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            for(int g = 0; g < layer.gameObjects.Length; g++)
            {
                Vector2 camPos = Camera.main.transform.position;
                Vector2 newPos = layer.startPositions[g] + (camPos - layer.startPositions[g]) * ((layer.distance) / 500F);
                if (lockYAxis)
                    layer.gameObjects[g].transform.position = new Vector2(newPos.x, start.y);
                else layer.gameObjects[g].transform.position = newPos;
            }
        }
    }

    public void addGameObject(GameObject gO, int layerNum){
        if(layerNum >= 0 && layerNum < parallaxLayers.Length) {
            int newLength = parallaxLayers[layerNum].gameObjects.Length + 1;
            Array.Resize<GameObject>(ref parallaxLayers[layerNum].gameObjects, newLength);
            Array.Resize<Vector2>(ref parallaxLayers[layerNum].startPositions, newLength);
            parallaxLayers[layerNum].gameObjects[newLength-1] = gO;
            parallaxLayers[layerNum].startPositions[newLength-1] = gO.transform.position;

            Debug.Log(parallaxLayers[layerNum].gameObjects.Length);
        }
    }
}

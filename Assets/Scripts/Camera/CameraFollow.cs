using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject follow;
    public GameObject lastFollow;
    public bool targetLocked;
    public Vector2 editorReferenceResolution;
    public float smoothnessX;
    public float smoothnessY;
    public float smoothnessZoom;
    public Vector2 customZoom;
    public float customSmoothnessZoom;
    public float zoomMultiplier = 1;
    public bool zoomMultiplierOverride;

    // Start is called before the first frame update
    void Start()
    {
        lastFollow = follow;
        if (Screen.height > Screen.width && !zoomMultiplierOverride) {
            zoomMultiplier = ((float) Screen.height / Screen.width) / (editorReferenceResolution.y / editorReferenceResolution.x);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 followPos = follow.transform.position;
        Vector3 newCameraPos = new Vector3(Mathf.Lerp(this.transform.position.x, followPos.x, Time.deltaTime * smoothnessX), Mathf.Lerp(this.transform.position.y, followPos.y, Time.deltaTime * smoothnessY), -10);
        this.transform.position = newCameraPos;
        Rigidbody2D rb;
        if (follow.TryGetComponent<Rigidbody2D>(out rb))
            GetComponent<Camera>().orthographicSize = getCameraZoom(rb.velocity.magnitude);
        else GetComponent<Camera>().orthographicSize = getCameraZoom(0);

        if (targetLocked && follow != lastFollow)
            follow = lastFollow;
        else lastFollow = follow;
    }

    private float getCameraZoom(float speed) {
        float movingZoom = (customZoom.x == 0 && customZoom.y == 0) ? 4 : customZoom.y;
        movingZoom *= zoomMultiplier;
        float stationaryZoom = (customZoom.x == 0 && customZoom.y == 0) ? 2.2F : customZoom.x;
        stationaryZoom *= zoomMultiplier;

        if (speed >= 0.5F)
            return Mathf.Lerp(GetComponent<Camera>().orthographicSize, movingZoom, Time.deltaTime * (customSmoothnessZoom == 0 ? smoothnessZoom : customSmoothnessZoom));
        else return Mathf.Lerp(GetComponent<Camera>().orthographicSize, stationaryZoom, Time.deltaTime * (customSmoothnessZoom == 0 ? smoothnessZoom : customSmoothnessZoom) * 2);
    }
}

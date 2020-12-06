using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    public float frequency;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angles = transform.localEulerAngles;
        angles.z += Time.deltaTime * frequency * 360;
        transform.localEulerAngles = angles;
    }
}

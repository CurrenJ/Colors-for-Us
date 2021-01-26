using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FrameGrabber : MonoBehaviour
{
    public bool saveFrames;
    public int frameCounter;
    public string folderName;
    // Start is called before the first frame update
    void Start()
    {
        folderName = System.DateTime.Now.ToFileTime().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        if(saveFrames)
            RTImage(Camera.main);
    }

    void RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
 
        string folder = "D:/Users/jeand/OneDrive/Documents/CFU-Pics/Frames/" + folderName;
        if(!Directory.Exists(folder))
            Directory.CreateDirectory(folder); 
        File.WriteAllBytes(folder + "/" + frameCounter + ".png", bytes);
        frameCounter++;
    }
}

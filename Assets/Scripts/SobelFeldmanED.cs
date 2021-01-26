using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SobelFeldmanED : MonoBehaviour
{
    private Texture2D tex;
    private Texture2D edge;
    public bool saveAsAsset;
    // Start is called before the first frame update
    void Start()
    {
        if (saveAsAsset) {
            SaveTextureAsPNG(detectEdges(), "D:/Users/jeand/OneDrive/Documents/Rotary-Re-done/Colors For Us Fixed/Assets/Images/MB Stuff/"  + tex.name + "_edges.png");
        }
    }
    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }

    private Texture2D detectEdges() {
        SpriteRenderer _Renderer;
        if (this.TryGetComponent<SpriteRenderer>(out _Renderer))
        {
            this.tex = _Renderer.sprite.texture;
            edge = new Texture2D(tex.width, tex.height);


            int[,] gx = new int[,] {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1}
            };

            int[,] gy = new int[,] {
                { 1, 2, 1 },
                { 0, 0, 0 },
                { -1, -2, -1}
            };

            float[,] Gx = convolute(tex, gx);
            float[,] Gy = convolute(tex, gy);
            float[,] gradient = magnitude(Gx, Gy);

            _Renderer.sprite = getSprite(gradient);
        }
        return _Renderer.sprite.texture;
    }

    private Sprite getSprite(float[,] grayscalemap) {
        Texture2D image = new Texture2D(grayscalemap.GetLength(1), grayscalemap.GetLength(0));
        for (int y = 0; y < grayscalemap.GetLength(0); y++) {
            for (int x = 0; x < grayscalemap.GetLength(1); x++) {
                float val = grayscalemap[y, x];
                Color c = Color.HSVToRGB(0, 0, val);
                c.a = val;
                image.SetPixel(x, y, c);

                if (x % 10 == 0 && y % 10 == 0)
                    Debug.Log(val + " (" + x + ", " + y + ")");

            }
        }
        image.Apply();
        return Sprite.Create(image, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5F, 0.5F), image.width);
    }

    private float[,] magnitude(float[,] a, float[,] b) {
        float[,] output = new float[a.GetLength(0), a.GetLength(1)];
        for (int y = 0; y < output.GetLength(0); y++) {
            for (int x = 0; x < output.GetLength(1); x++) {
                output[y, x] = Mathf.Sqrt(Mathf.Pow(a[y, x], 2) + Mathf.Pow(b[y, x], 2));
            }
        }
        return output;
    }

    private float[,] convolute(Texture2D image, int[,] kernel) {
        float[,] output = new float[image.height, image.width];
        for (int iY = 0; iY < image.height; iY++) {
            for (int iX = 0; iX < image.width; iX++) {
                float accumulator = 0;

                for (int kY = 0; kY < kernel.GetLength(0); kY++) {
                    for (int kX = 0; kX < kernel.GetLength(1); kX++) {
                        if (iY + kY - 1 < image.height && iY + kY - 1 >= 0 && iX + kX - 1 < image.width && iX + kX - 1 >= 0) {
                            accumulator += image.GetPixel(iX + kX - 1, iY + kY - 1).grayscale * kernel[kY,kX];
                        }
                    }
                }

                //if (iX % 10 == 0 && iY % 10 == 0)
                //    Debug.Log(accumulator + " (" + iX + ", " + iY + ")");

                output[iY,iX] = accumulator;
            }
        }
        return output;
    }
}

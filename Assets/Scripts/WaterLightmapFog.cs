using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class WaterLightmapFog : MonoBehaviour
{
    public float fogDensity;
    public Color fogColor;
    public Color baseColor;
    public float baseMultBlurPixels;
    public float blurOverDrive;
    public float depthAmbient;
    public Vector3 terrainSize;
    public Collider terrainCollider;
    public Texture2D texture;

    [UnityEngine.ContextMenu("Apply Fog")]
    public virtual void ApplyFog()
    {
        RaycastHit hit = default(RaycastHit);
        float lerp = 0.0f;
        Texture2D bColorTex = new Texture2D(this.texture.width, this.texture.height);
        float x = 0f;
        float y = 0f;
        while (x < this.texture.width)
        {
            y = 0f;
            while (y < this.texture.height)
            {
                Vector3 vect = new Vector3( (float) (x / this.texture.width) * this.terrainSize.x, 400f, (float) (y / this.texture.height) * this.terrainSize.y);
                if (this.terrainCollider.Raycast(new Ray(vect, Vector3.up * -500), out hit, 500))
                {
                    float depth = 35.35f - hit.point.y;
                    if (x == 256)
                    {
                        MonoBehaviour.print(vect);
                    }
                    if (depth > 0)
                    {
                        Color lightCol = this.texture.GetPixel((int) x, (int) y);
                        Color curCol = Color.Lerp(lightCol, Color.gray, (this.depthAmbient * depth) * this.fogDensity);
                        Vector3 fog = new Vector3(Mathf.Pow(this.fogColor.r, depth * this.fogDensity), Mathf.Pow(this.fogColor.g, depth * this.fogDensity), Mathf.Pow(this.fogColor.b, depth * this.fogDensity));
                        this.texture.SetPixel((int) x, (int) y, new Color((curCol.r * fog.x) * lightCol.a, (curCol.g * fog.y) * lightCol.a, (curCol.b * fog.z) * lightCol.a, curCol.a));
                        bColorTex.SetPixel((int) x, (int) y, new Color(this.baseColor.r, this.baseColor.g, this.baseColor.b, 1));
                    }
                    else
                    {
                        bColorTex.SetPixel((int) x, (int) y, Color.white);
                    }
                }
                y++;
            }
            x++;
        }
        //bColorTex.Apply();
        x = 0f;
        float pix = 0f;
        while (x < this.texture.width)
        {
            y = 0f;
            while (y < this.texture.height)
            {
                Color curCol = this.texture.GetPixel((int) x, (int) y);
                if (this.baseMultBlurPixels > 0)
                {
                    lerp = (1f / (4f * this.baseMultBlurPixels)) * (1 + this.blurOverDrive);
                    pix = this.baseMultBlurPixels;
                }
                else
                {
                    lerp = 1f;
                    pix = this.baseMultBlurPixels;
                }
                Color temp = bColorTex.GetPixel((int) Mathf.Clamp(x, 0, this.texture.width - 1), (int) Mathf.Clamp(y, 0, this.texture.width - 1));
                curCol = Color.Lerp(curCol, new Color(curCol.r * temp.r, curCol.g * temp.g, curCol.b * temp.b, curCol.a), lerp);
                while (pix > 0)
                {
                    temp = bColorTex.GetPixel((int) Mathf.Clamp(x + pix, 0, this.texture.width - 1), (int) Mathf.Clamp(y, 0, this.texture.width - 1));
                    curCol = Color.Lerp(curCol, new Color(curCol.r * temp.r, curCol.g * temp.g, curCol.b * temp.b, curCol.a), lerp);
                    temp = bColorTex.GetPixel((int) Mathf.Clamp(x - pix, 0, this.texture.width - 1), (int) Mathf.Clamp(y, 0, this.texture.width - 1));
                    curCol = Color.Lerp(curCol, new Color(curCol.r * temp.r, curCol.g * temp.g, curCol.b * temp.b, curCol.a), lerp);
                    temp = bColorTex.GetPixel((int) Mathf.Clamp(x, 0, this.texture.width - 1), (int) Mathf.Clamp(y + pix, 0, this.texture.width - 1));
                    curCol = Color.Lerp(curCol, new Color(curCol.r * temp.r, curCol.g * temp.g, curCol.b * temp.b, curCol.a), lerp);
                    temp = bColorTex.GetPixel((int) Mathf.Clamp(x, 0, this.texture.width - 1), (int) Mathf.Clamp(y - pix, 0, this.texture.width - 1));
                    curCol = Color.Lerp(curCol, new Color(curCol.r * temp.r, curCol.g * temp.g, curCol.b * temp.b, curCol.a), lerp);
                    pix--;
                }
                this.texture.SetPixel((int) x, (int) y, curCol);
                y++;
            }
            x++;
        }
        this.texture.Apply();
        UnityEngine.Object.DestroyImmediate(bColorTex);
    }

    public WaterLightmapFog()
    {
        this.depthAmbient = 1.5f;
    }

}
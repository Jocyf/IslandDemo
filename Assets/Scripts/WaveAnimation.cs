using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class WaveAnimation : MonoBehaviour
{
    public GameObject[] siblings;
    public int index;
    public float offset;
    public float slideMin;
    public float slideMax;
    public float slideSpeed;
    public float slideSharpness;
    public float scaleMin;
    public float scaleMax;
    public float scaleSpeed;
    public float scaleSharpness;
    public float fadeSpeed;
    public Vector3 baseScroll;
    public float baseRotation;
    public Vector3 baseScale;
    private Material theMaterial;
    private float slide;
    private float slideInertia;
    private float scale;
    private float scaleInertia;
    private Vector3 basePos;
    private Vector3 texScale;
    private float lastSlide;
    private float fade;
    private Color color;
    private Color fadeColor;
    public WaveAnimation original;
    public virtual void Start()
    {
        this.CheckHWSupport();
        object[] waves = new object[0];
        waves = this.GetComponents(typeof(WaveAnimation));
        if ((waves.Length == 1) && (this.original == null))
        {
            this.original = this;
        }
        foreach (GameObject s in this.siblings)
        {
            this.AddCopy(s, this.original, false);
        }
        if (waves.Length < this.GetComponent<Renderer>().materials.Length)
        {
            this.AddCopy(this.gameObject, this.original, true);
        }
        this.theMaterial = this.GetComponent<Renderer>().materials[this.index];
        this.color = this.theMaterial.GetColor("_Color");
        this.fadeColor = this.color;
        this.fadeColor.a = 0;
        this.texScale = this.theMaterial.GetTextureScale("_MainTex");
    }

    private void CheckHWSupport()
    {
        bool supported = this.GetComponent<Renderer>().sharedMaterial.shader.isSupported;
        foreach (GameObject s in this.siblings)
        {
            s.GetComponent<Renderer>().enabled = supported;
        }
        this.GetComponent<Renderer>().enabled = supported;
    }

    public virtual void Update()
    {
        this.CheckHWSupport();
        this.slideInertia = Mathf.Lerp(this.slideInertia, Mathf.PingPong((Time.time * this.scaleSpeed) + this.offset, 1), this.slideSharpness * Time.deltaTime);
        this.slide = Mathf.Lerp(this.slide, this.slideInertia, this.slideSharpness * Time.deltaTime);
        this.theMaterial.SetTextureOffset("_MainTex", new Vector3(this.index * 0.35f, Mathf.Lerp(this.slideMin, this.slideMax, this.slide) * 2, 0));
        this.theMaterial.SetTextureOffset("_Cutout", new Vector3(this.index * 0.79f, Mathf.Lerp(this.slideMin, this.slideMax, this.slide) / 2, 0));
        this.fade = Mathf.Lerp(this.fade, (this.slide - this.lastSlide) > 0 ? 0.3f : 0, Time.deltaTime * this /**/.fadeSpeed);
        this.lastSlide = this.slide;
        this.theMaterial.SetColor("_Color", Color.Lerp(this.fadeColor, this.color, this.fade));
        this.scaleInertia = Mathf.Lerp(this.scaleInertia, Mathf.PingPong((Time.time * this.scaleSpeed) + this.offset, 1), this.scaleSharpness * Time.deltaTime);
        this.scale = Mathf.Lerp(this.scale, this.scaleInertia, this.scaleSharpness * Time.deltaTime);
        this.theMaterial.SetTextureScale("_MainTex", new Vector3(this.texScale.x, Mathf.Lerp(this.scaleMin, this.scaleMax, this.scale), this.texScale.z));
        this.basePos = this.basePos + (this.baseScroll * Time.deltaTime);
        Vector3 inverseScale = new Vector3(1 / this.baseScale.x, 1 / this.baseScale.y, 1 / this.baseScale.z);
        Matrix4x4 uvMat = Matrix4x4.TRS(this.basePos, Quaternion.Euler(this.baseRotation, 90, 90), inverseScale);
        this.theMaterial.SetMatrix("_WavesBaseMatrix", uvMat);
    }

    public virtual void AddCopy(GameObject ob, WaveAnimation original, bool copy)
    {
        WaveAnimation newWave = (WaveAnimation) ob.AddComponent(typeof(WaveAnimation));
        newWave.original = original;
        if (copy)
        {
            newWave.index = this.index + 1;
        }
        else
        {
            newWave.index = this.index;
        }
        newWave.offset = original.offset + (2f / (float) GetComponent<Renderer>().materials.Length);
        newWave.slideMin = original.slideMin;
        newWave.slideMax = original.slideMax;
        newWave.slideSpeed = original.slideSpeed + Random.Range(-original.slideSpeed / 5, original.slideSpeed / 5);
        newWave.slideSharpness = original.slideSharpness + Random.Range(-original.slideSharpness / 5, original.slideSharpness / 5);
        newWave.scaleMin = original.scaleMin;
        newWave.scaleMax = original.scaleMax;
        newWave.scaleSpeed = original.scaleSpeed + Random.Range(-original.scaleSpeed / 5, original.scaleSpeed / 5);
        newWave.scaleSharpness = original.scaleSharpness + Random.Range(-original.scaleSharpness / 5, original.scaleSharpness / 5);
        newWave.fadeSpeed = original.fadeSpeed;
        Vector3 randy = Random.onUnitSphere;
        randy.y = 0;
        newWave.baseScroll = randy.normalized * original.baseScroll.magnitude;
        newWave.baseRotation = Random.Range(0, 360);
        newWave.baseScale = original.baseScale * Random.Range(0.8f, 1.2f);
    }

    public WaveAnimation()
    {
        this.siblings = new GameObject[0];
        this.slideMin = -0.1f;
        this.slideMax = 0.4f;
        this.slideSpeed = 0.5f;
        this.slideSharpness = 1f;
        this.scaleMin = 1f;
        this.scaleMax = 0.4f;
        this.scaleSpeed = 0.5f;
        this.scaleSharpness = 0.5f;
        this.baseScroll = new Vector3(0.1f, 0, 0.3547f);
        this.baseScale = new Vector3(10f, 10, 10f);
        this.fade = 1f;
    }

}
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Water;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
public partial class UnderwaterEffects : MonoBehaviour
{
    public Water water;
    public float waterLevel;
    public AudioClip uAudio;
    public AudioClip aAudio;
    public Color uColor;
    public float uDensity;
    public Color aColor;
    public float aDensity;
    public Renderer waterSurface;
    public Renderer underwaterSurface;
    private bool below;

    private PostProcessVolume _volume;
    private Bloom bloom;
    private DepthOfField dof;

    //private var glow : GlowEffectIsland;
    //private var blur : BlurEffectIsland;
    public virtual void Awake()
    {
        if(!water)
	    {
		    water = FindObjectOfType<Water>();
		    if(water) waterLevel = water.transform.position.y;
         
	    }
        this.waterLevel = this.water.transform.position.y;
        this.aColor = RenderSettings.fogColor;
        this.aDensity = RenderSettings.fogDensity;

        _volume = FindObjectOfType<PostProcessVolume>();
        _volume.profile.TryGetSettings(out bloom);
        _volume.profile.TryGetSettings(out dof);
	    if( !bloom || !dof)
	    {
		    Debug.LogError("no right Glow/Blur assigned to camera!");
		    enabled = false;
	    }
        if (!this.waterSurface || !this.underwaterSurface)
        {
            Debug.LogError("assign water & underwater surfaces");
            this.enabled = false;
        }
        if (this.underwaterSurface != null)
        {
            this.underwaterSurface.enabled = false; // initially underwater is disabled
        }
    }

    public virtual void Update()
    {
        if ((this.waterLevel < this.transform.position.y) && this.below)
        {
            this.GetComponent<AudioSource>().clip = this.aAudio;
            this.GetComponent<AudioSource>().Play();
            RenderSettings.fogDensity = this.aDensity;
            RenderSettings.fogColor = this.aColor;
            this.below = false;
            bloom.active = false;
            dof.active = false;
            this.waterSurface.enabled = true;
            this.underwaterSurface.enabled = false;
        }
        if ((this.waterLevel > this.transform.position.y) && !this.below)
        {
            this.GetComponent<AudioSource>().clip = this.uAudio;
            this.GetComponent<AudioSource>().Play();
            RenderSettings.fogDensity = this.uDensity;
            RenderSettings.fogColor = this.uColor;
            this.below = true;
            bloom.active = true;
            dof.active = true;
            this.waterSurface.enabled = false;
            this.underwaterSurface.enabled = true;
        }
    }

    public UnderwaterEffects()
    {
        this.uColor = new Color(1, 1, 1, 1);
        this.uDensity = 0.05f;
        this.aColor = new Color(1, 1, 1, 1);
        this.aDensity = 0.008f;
    }

}
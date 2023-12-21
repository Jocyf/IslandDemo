using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityStandardAssets.Water;

[System.Serializable]
public partial class PerformanceTweak : MonoBehaviour
{
    public FPSCounter fpsCounter;
	public Water water;
    public Terrain terrain;
    public float messageTime;
    public float scrollTime;

	[Space(10)]
	public bool tweakWater = true;
    public bool tweakTerrain = true;

    [Space(10)]
	[SerializeField]
    private List<string> messages = new List<string>();
	[SerializeField]
	private List<float> times = new List<float>();
    private float lastTime;
    private bool doneNotes;
    private float origDetailDist;
    private float origSplatDist;
    private float origTreeDist;
    private int origMaxLOD;
    private bool softVegetationOff;
    private bool splatmapsOff;
    private float lowFPS;
    private float skipChangesTimeout;
    private int nextTerrainChange;

	private void Start()
	{
		if( !fpsCounter || !water || !terrain ) {
			Debug.LogWarning("Some of performance objects are not set up");
			enabled = false;
			return;
		}
	
		origDetailDist = terrain.detailObjectDistance;
		origSplatDist = terrain.basemapDistance;
		origTreeDist = terrain.treeDistance;
		origMaxLOD = terrain.heightmapMaximumLOD;
		skipChangesTimeout = 0.0f;

		Init();
	}

	private void Update ()
	{
		if (!fpsCounter || !water || !terrain) { return; }	// Security Sentence
		
		if( !doneNotes && !Application.isEditor )
		{
			var hwWater = water.FindHardwareWaterSupport();
			if( hwWater == Water.WaterMode.Simple )
			{
				AddMessage( "Note: water reflections not supported on this computer" );
				// in this case it also happens that terrain splat maps are not supported :)
				AddMessage( "Note: high detail terrain textures not supported on this computer" );
				splatmapsOff = true;
			}
			if( hwWater == Water.WaterMode.Reflective )
				AddMessage( "Note: water refractions not supported on this computer" );
			
			var gfxCard = SystemInfo.graphicsDeviceName.ToLower();
			var gfxVendor = SystemInfo.graphicsDeviceVendor.ToLower();
			if( gfxVendor.Contains("intel") )
			{
				// on pre-GMA950, increase fog and reduce far plane by 4x :)
				if( hwWater == Water.WaterMode.Simple )
				{
					ReduceDrawDistance( 4.0f, "Note: reducing draw distance (old Intel video card detected)" );
				}
			
				softVegetationOff = true;
				QualitySettings.softVegetation = false;
				AddMessage( "Note: turning off soft vegetation (Intel video card detected)" );
			}
			else if( gfxVendor == "sis" )
			{
				softVegetationOff = true;
				QualitySettings.softVegetation = false;
				AddMessage( "Note: turning off soft vegetation (SIS video card detected)" );
			}
			else if( gfxCard.Contains("geforce") && (
				gfxCard.Contains("5200") || gfxCard.Contains("5500") || gfxCard.Contains("6100") || hwWater == Water.WaterMode.Simple) )
			{
				// on slow/old geforce cards, increase fog and reduce far plane by 2x
				ReduceDrawDistance( 2.0f, "Note: reducing draw distance (slow GeForce card detected)" );
			
				softVegetationOff = true;
				QualitySettings.softVegetation = false;
				AddMessage( "Note: turning off soft vegetation (slow GeForce card detected)" );
			}
			else
			{
				// on other old cards, increase fog and reduce far plane by 2x
				if( hwWater == Water.WaterMode.Simple )
				{
					ReduceDrawDistance( 2.0f, "Note: reducing draw distance (old video card detected)" );
				}
			}
		
			skipChangesTimeout = 0.0f;
			doneNotes = true;
		}
	
		DoTweaks();
	
		UpdateMessages();
	}

	private void ReduceDrawDistance( float factor, string message)
	{
		AddMessage( message );
		UnderwaterEffects underwater;
		underwater = FindObjectOfType<UnderwaterEffects>();
		RenderSettings.fogDensity *= factor;
		if( underwater )
		{
			underwater.uDensity *= factor;
			underwater.aDensity *= factor;
		}
		Camera.main.farClipPlane /= factor;
	}

	private void OnDisable()
	{
		QualitySettings.softVegetation = true;
	}

	private void DoTweaks()
	{
		if( !fpsCounter.HasFPS() )
			return; // enough time did not pass yet to get decent FPS count		

		Water.WaterMode hwWater = water.FindHardwareWaterSupport();
		float fps  = fpsCounter.GetFPS();
	
		// don't do too many adjustments at time... allow one per
		// FPS update interval
		skipChangesTimeout -= Time.deltaTime;
		if( skipChangesTimeout < 0.0f )
			skipChangesTimeout = 0.0f;
		if( skipChangesTimeout > 0.0f )
			return;

		// water tweaks
		if (tweakWater)
		{
			Water.WaterMode curWater = water.GetWaterMode();
			if (fps > 70.0)
			{
				// frame rate high, use refractions | reflections
				if (hwWater == Water.WaterMode.Refractive && curWater < Water.WaterMode.Refractive)
				{
					water.waterMode = Water.WaterMode.Refractive;
					AddMessage("Framerate high, turning water refractions on");
					return;
				}
				if (hwWater == Water.WaterMode.Reflective && curWater < Water.WaterMode.Reflective)
				{
					water.waterMode = Water.WaterMode.Reflective;
					AddMessage("Framerate high, turning water reflections on");
					return;
				}
			}
			else if (fps > 30.0)
			{
				// frame rate sort of high, can use water reflections
				if (hwWater > Water.WaterMode.Simple && curWater == Water.WaterMode.Simple)
				{
					water.waterMode = Water.WaterMode.Reflective;
					AddMessage("Framerate ok, turning water reflections on");
					return;
				}
				if (hwWater == Water.WaterMode.Refractive && curWater == Water.WaterMode.Refractive)    /**/
				{
					water.waterMode = Water.WaterMode.Reflective;
					AddMessage("Framerate high, turning water refractions on");
					return;
				}
			}
			else if (fps < 25.0)
			{
				// frame rate sort of low, use reflections only
				if (curWater > Water.WaterMode.Reflective)
				{
					water.waterMode = Water.WaterMode.Reflective;
					AddMessage("Framerate low, turning water refractions off");
					return;
				}
			}
			else if (fps < 10.0)
			{
				// frame rate very low, turn water to simple
				if (curWater > Water.WaterMode.Simple)
				{
					water.waterMode = Water.WaterMode.Simple;
					AddMessage("Framerate very low, turning water reflections off");
					return;
				}
			}
		}

		// terrain tweaks
		if (tweakTerrain)
		{
			if (fps > 25.0)
			{
				// bump up!
				++nextTerrainChange;
				if (nextTerrainChange >= 4)
					nextTerrainChange = 0;

				if (nextTerrainChange == 0 && terrain.detailObjectDistance < origDetailDist)
				{
					terrain.detailObjectDistance *= 2.0f;
					if (!softVegetationOff)
						QualitySettings.softVegetation = true;
					AddMessage("Framerate ok, increasing vegetation detail");
					return;
				}
				if (nextTerrainChange == 1 && !splatmapsOff && terrain.basemapDistance < origSplatDist)
				{
					terrain.basemapDistance *= 2.0f;
					AddMessage("Framerate ok, increasing terrain texture detail");
					return;
				}
				if (nextTerrainChange == 2 && terrain.treeDistance < origTreeDist)
				{
					terrain.treeDistance *= 2.0f;
					AddMessage("Framerate ok, increasing tree draw distance");
					return;
				}
				if (nextTerrainChange == 3 && terrain.heightmapMaximumLOD > origMaxLOD)
				{
					--terrain.heightmapMaximumLOD;
					AddMessage("Framerate ok, increasing terrain detail");
					return;
				}
			}
			if (fps < lowFPS)
			{
				// lower it
				++nextTerrainChange;
				if (nextTerrainChange >= 4)
				{
					nextTerrainChange = 0;
					lowFPS = 10.0f; // ok, this won't be fast...
				}

				if (nextTerrainChange == 0 && terrain.detailObjectDistance >= origDetailDist / 16.0)
				{
					terrain.detailObjectDistance *= 0.5f;
					QualitySettings.softVegetation = false;
					AddMessage("Framerate low, reducing vegetation detail");
					return;
				}
				if (nextTerrainChange == 1 && !splatmapsOff && terrain.basemapDistance >= origSplatDist / 16.0)
				{
					terrain.basemapDistance *= 0.5f;
					AddMessage("Framerate low, reducing terrain texture detail");
					return;
				}
				if (nextTerrainChange == 2 && terrain.treeDistance >= origTreeDist / 16.0)
				{
					terrain.treeDistance *= 0.5f;
					AddMessage("Framerate low, reducing tree draw distance");
					return;
				}
				if (nextTerrainChange == 3 && terrain.heightmapMaximumLOD < 1)
				{
					++terrain.heightmapMaximumLOD;
					AddMessage("Framerate low, reducing terrain detail");
					return;
				}
			}
		}
	}

	private void AddMessage( string _msg )
	{
		messages.Add(_msg);
		times.Add( messageTime );
		lastTime = scrollTime;
		skipChangesTimeout = fpsCounter.updateInterval * 3.0f;
	}

	private void UpdateMessages()
	{
		float dt = Time.deltaTime;
		for(int i = 0; i < times.Count; i++) { times[i] -= dt; }        //foreach (float t in times) { t -= dt; }

		while ( times.Count > 0 && times[0] < 0.0 ) {
			times.RemoveAt(0);      //times.Shift();
			messages.RemoveAt(0);   //messages.Shift();	
		}
		lastTime -= dt;
		if (lastTime < 0.0) { lastTime = 0.0f; }
	}

	private void OnGUI()
	{
		int height = 15;
		int n = messages.Count;
		Rect rc = new Rect( 2, Screen.height - 2 - n * height + (lastTime/scrollTime*height), 600, 20 );
		for( var i = 0; i < n; ++i )
		{
			string text = messages[i];
			float time = times[i];
			var alpha = time / messageTime;

			Color _color = GUI.color;
			if ( alpha < 0.2 )
				_color.a = alpha / 0.2f;
			else if( alpha > 0.9 )
				_color.a = 1.0f - (alpha-0.9f) / (1-0.9f);
			else
				_color.a = 1.0f;

			GUI.color = _color;
			GUI.Label( rc, text );
			rc.y += height;
		}
	}

	private void Init()
    {
        this.messageTime = 10f;
        this.scrollTime = 0.7f;
		//this.messages = new List<string>();
		messages.Clear();
		//this.times = new List<float>();
		times.Clear();
		this.lowFPS = 15f;
        this.skipChangesTimeout = 1f;
    }

}
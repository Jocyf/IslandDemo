using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public partial class FPSCounter : MonoBehaviour
{
    // Attach this to a UI.Text to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.
    public float updateInterval = 1f;
    private float accum; // FPS accumulated over the interval
    private int frames; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float fps = 15; // Current FPS
    private double lastSample;
    private int gotIntervals;
    private Text myText;

    public virtual void Start()
    {
        updateInterval = 1f;

        timeleft = updateInterval;
        lastSample = Time.realtimeSinceStartup;
        myText = GetComponent<Text>();
    }

    public virtual float GetFPS() { return fps; }

    public virtual bool HasFPS() { return gotIntervals > 2; }

    public virtual void Update()
    {
        ++frames;
        float newSample = Time.realtimeSinceStartup;
        double deltaTime = newSample - lastSample;
        lastSample = newSample;
        timeleft = (float) (timeleft - deltaTime);
        accum = (float) (accum + (1f / deltaTime));
        // Interval ended - update UI.Text and start new interval
        if (timeleft <= 0f)
        {
             // display two fractional digits (f2 format)
            fps = Mathf.FloorToInt(accum / frames);
            myText.text = fps.ToString();
            timeleft = updateInterval;
            accum = 0f;
            frames = 0;
            ++gotIntervals;
        }
    }

}
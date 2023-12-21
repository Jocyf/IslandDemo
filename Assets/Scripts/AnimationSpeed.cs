using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AnimationSpeed : MonoBehaviour
{
    public string statename;
    public float speed;
    public Animation _animation;
    public virtual void Start()
    {
        this._animation.GetComponent(typeof(Animation));
        this._animation[this.statename].speed = this.speed;
    }

    public AnimationSpeed()
    {
        this.statename = "defaulto";
        this.speed = 0.5f;
    }

}
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TimedObjectDestructor : MonoBehaviour
{
    public float timeOut;
    public bool detachChildren;
    public virtual void Awake()
    {
        this.Invoke("DestroyNow", this.timeOut);
    }

    public virtual void DestroyNow()
    {
        if (this.detachChildren)
        {
            this.transform.DetachChildren();
        }
        Destroy(this.gameObject);
    }

    public TimedObjectDestructor()
    {
        this.timeOut = 1f;
    }

}
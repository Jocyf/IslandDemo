using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.AddComponentMenu("Camera-Control/Smooth Look At")]
public partial class SmoothLookAt : MonoBehaviour
{
    public Transform target;
    public float damping;
    public bool smooth;
    public virtual void LateUpdate()
    {
        if (this.target)
        {
            if (this.smooth)
            {
                 // Look at and dampen the rotation
                Quaternion rotation = Quaternion.LookRotation(this.target.position - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * this.damping);
            }
            else
            {
                 // Just lookat
                this.transform.LookAt(this.target);
            }
        }
    }

    public virtual void Start()
    {
        // Make the rigid body not change rotation
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    public SmoothLookAt()
    {
        this.damping = 6f;
        this.smooth = true;
    }

}